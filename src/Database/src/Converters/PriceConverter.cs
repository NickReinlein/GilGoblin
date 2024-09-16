using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId);
}

public class PriceConverter(
    GilGoblinDbContext dbContext,
    IQualityPriceDataConverter qualityConverter,
    ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId)
    {
        try
        {
            // 1. take the web poco 
            // 2. take hq and nq separately as QualityPriceDataPoco
            // 3. take 3 price types from each of 2 QualityPriceDataPoco as PriceDataPointWebPoco
            // 4. take 3 regions from each PriceDataPointWebPoco as PriceDataDetailPoco
            // 5. save each region data point
            // 6. take the 3 ids returned and assign them to a new PriceDataPointPoco
            // 7. do this for 3 price types into a new PricePoco
            // 8. do this for hq and nq for 2 new pricePocos

            // PriceWebPoco (includes hq and nq)
            // 2 QualityPriceDataPoco >> (2 PricePoco later)
            // 3 PriceDataPointWebPoco >> 
            // 1 PricePoco 
            // => 2 PricePocos total, hq + nq

            if (webPoco is null || worldId < 1)
                throw new ArgumentException("Invalid world id", nameof(worldId));

            var itemId = webPoco.ItemId;
            var nqPrices = await qualityConverter.ConvertAsync(webPoco.Nq, itemId, false);
            var nq = GetPricePocoFromQualityPrices(worldId, nqPrices, itemId);

            var hqPrices = await qualityConverter.ConvertAsync(webPoco.Hq, itemId, false);
            var hq = GetPricePocoFromQualityPrices(worldId, hqPrices, itemId);


            if (hq is null && nq is null)
                return (null, null);

            if (hq is not null)
                dbContext.Price.Add(hq);

            if (nq is not null)
                dbContext.Price.Add(nq);

            await dbContext.SaveChangesAsync();

            return (hq, nq);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return (null, null);
        }
    }

    private static PricePoco? GetPricePocoFromQualityPrices(int worldId, QualityPriceDataPoco? nqPrices, int itemId)
    {
        return nqPrices is null
            ? null
            : new PricePoco
            {
                ItemId = itemId,
                WorldId = worldId,
                IsHq = false,
                Updated = DateTimeOffset.Now,
                AverageSalePriceId = nqPrices?.AverageSalePrice?.Id ?? 0,
                MinListingId = nqPrices?.MinListing?.Id ?? 0,
                RecentPurchaseId = nqPrices?.RecentPurchase?.Id ?? 0,
                DailySaleVelocityId = nqPrices?.DailySaleVelocity?.Id ?? 0
            };
    }
}