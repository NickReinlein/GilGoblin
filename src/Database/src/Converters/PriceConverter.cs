using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId);
}

public class PriceConverter(GilGoblinDbContext dbContext, ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?,PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId)
    {
        try
        {
            var hqPrice = await GetPricePocoForGivenQuality(webPoco, worldId, true);
            var nqPrice = await GetPricePocoForGivenQuality(webPoco, worldId, false);
            return (hqPrice, nqPrice);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to convert price data");
            return (null, null);
        }
    }

    private async Task<PricePoco> GetPricePocoForGivenQuality(PriceWebPoco webPoco, int worldId, bool isHq)
    {
        var pricePoco = new PricePoco
        {
            ItemId = webPoco.ItemId, WorldId = worldId, IsHq = isHq, Updated = DateTimeOffset.UtcNow
        };

        if (isHq && webPoco.Hq != null)
        {
            await MapQualityDataAsync(webPoco.Hq, pricePoco);
        }
        else if (!isHq && webPoco.Nq != null)
        {
            await MapQualityDataAsync(webPoco.Nq, pricePoco);
        }
        else
        {
            throw new Exception("Failed to map price data: No Hq or Nq data found");
        }

        return pricePoco;
    }

    private async Task MapQualityDataAsync(QualityPriceDataPoco qualityData, PricePoco pricePoco)
    {
        if (qualityData.MinListing != null)
        {
            pricePoco.MinListingId = await GetOrCreatePriceDataAsync(qualityData.MinListing, "minListing");
        }

        if (qualityData.RecentPurchase != null)
        {
            pricePoco.RecentPurchaseId = await GetOrCreatePriceDataAsync(qualityData.RecentPurchase, "recentPurchase");
        }

        if (qualityData.AverageSalePrice != null)
        {
            pricePoco.AverageSalePriceId =
                await GetOrCreatePriceDataAsync(qualityData.AverageSalePrice, "averageSalePrice");
        }

        if (qualityData.DailySaleVelocity != null)
        {
            var dailySaleVelocity = new DailySaleVelocityPoco(
                0,
                pricePoco.ItemId,
                pricePoco.IsHq,
                qualityData.DailySaleVelocity.WorldQuantity,
                qualityData.DailySaleVelocity.DcQuantity,
                qualityData.DailySaleVelocity.RegionQuantity);

            await dbContext.DailySaleVelocity.AddAsync(dailySaleVelocity);
            await dbContext.SaveChangesAsync();
            pricePoco.DailySaleVelocityId = dailySaleVelocity.Id;
        }
    }

    private async Task<int?> GetOrCreatePriceDataAsync(PriceDataPointWebPoco dataPoint, string priceDataType)
    {
        if (!dataPoint.World.HasValidPrice() && !dataPoint.Dc.HasValidPrice() && !dataPoint.Region.HasValidPrice())
            return null;

        var priceDataList = new List<PriceDataDbPoco>
        {
            new(
                0,
                priceDataType,
                dataPoint.World?.Price ?? 0f,
                dataPoint.World?.Timestamp ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
                dataPoint.World?.WorldId ?? 0),
            new(
                0,
                priceDataType,
                dataPoint.Dc?.Price ?? 0f,
                dataPoint.Dc?.Timestamp ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
                dataPoint.Dc?.WorldId ?? 0),
            new(
                0,
                priceDataType,
                dataPoint.Region?.Price ?? 0f,
                dataPoint.Region?.Timestamp ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
                dataPoint.Region?.WorldId ?? 0),
        };

        await dbContext.PriceData.AddRangeAsync(priceDataList);
        await dbContext.SaveChangesAsync();
        return priceDataList.Count;
    }
}