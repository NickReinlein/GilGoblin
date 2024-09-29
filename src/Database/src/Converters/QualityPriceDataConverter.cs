using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IQualityPriceDataConverter
{
    Task<QualityPriceDataPoco?> ConvertAsync(QualityPriceDataWebPoco? qualityData, int itemId, bool isHq);
}

public class QualityPriceDataConverter(
    IServiceProvider serviceProvider,
    IPriceDataPointConverter dataPointConverter,
    IDailySaleVelocityConverter saleVelocityConverter,
    ILogger<QualityPriceDataConverter> logger)
    : IQualityPriceDataConverter
{
    public async Task<QualityPriceDataPoco?> ConvertAsync(QualityPriceDataWebPoco? qualityData, int itemId, bool isHq)
    {
        try
        {
            if (qualityData is null || itemId < 1 || !qualityData.HasValidPrice())
                return null;

            var minListing =
                await dataPointConverter.ConvertAndSaveAsync(qualityData.MinListing, itemId, isHq);
            var averageSalePrice =
                await dataPointConverter.ConvertAndSaveAsync(qualityData.AverageSalePrice, itemId, isHq);
            var recentPurchase =
                await dataPointConverter.ConvertAndSaveAsync(qualityData.RecentPurchase, itemId, isHq);
            var dailySaleVelocity =
                await saleVelocityConverter.ConvertAsync(qualityData.DailySaleVelocity, itemId, isHq);

            return await SaveToDatabaseAsync(itemId, isHq, minListing, averageSalePrice, recentPurchase,
                dailySaleVelocity);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }

    private async Task<QualityPriceDataPoco> SaveToDatabaseAsync(
        int itemId,
        bool isHq,
        PriceDataPointPoco? minListing,
        PriceDataPointPoco? averageSalePrice,
        PriceDataPointPoco? recentPurchase,
        DailySaleVelocityPoco? dailySaleVelocity)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

        MinListingPoco? minListingDb = null;
        AverageSalePricePoco? averageSalePriceDb = null;
        RecentPurchasePoco? recentPurchaseDb = null;
        DailySaleVelocityPoco? dailySaleVelocityDb = null;

        if (minListing is not null)
        {
            minListingDb = new MinListingPoco(
                itemId,
                isHq,
                minListing.WorldDataPointId,
                minListing.DcDataPointId,
                minListing.RegionDataPointId);
            dbContext.MinListing.Add(minListingDb);
        }

        if (averageSalePrice is not null)
        {
            averageSalePriceDb = new AverageSalePricePoco(
                itemId,
                isHq,
                averageSalePrice.WorldDataPointId,
                averageSalePrice.DcDataPointId,
                averageSalePrice.RegionDataPointId);
            dbContext.AverageSalePrice.Add(averageSalePriceDb);
        }

        if (recentPurchase is not null)
        {
            recentPurchaseDb = new RecentPurchasePoco(
                itemId,
                isHq,
                recentPurchase.WorldDataPointId,
                recentPurchase.DcDataPointId,
                recentPurchase.RegionDataPointId);
            dbContext.RecentPurchase.Add(recentPurchaseDb);
        }

        if (dailySaleVelocity is not null)
        {
            dailySaleVelocityDb = new DailySaleVelocityPoco(
                itemId,
                isHq,
                dailySaleVelocity.World,
                dailySaleVelocity.Dc,
                dailySaleVelocity.Region);
            dbContext.DailySaleVelocity.Add(dailySaleVelocityDb);
        }

        await dbContext.SaveChangesAsync();

        return new QualityPriceDataPoco(minListingDb, averageSalePriceDb, recentPurchaseDb, dailySaleVelocityDb);
    }
}