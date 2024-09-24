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
                await dataPointConverter.ConvertAsync(qualityData.MinListing, itemId, isHq);
            var averageSalePrice =
                await dataPointConverter.ConvertAsync(qualityData.AverageSalePrice, itemId, isHq);
            var recentPurchase =
                await dataPointConverter.ConvertAsync(qualityData.RecentPurchase, itemId, isHq);
            if (qualityData.DailySaleVelocity.HasAValidQuantity())
                logger.LogWarning("Found one");

            var dailySaleVelocity =
                await saleVelocityConverter.ConvertAsync(qualityData.DailySaleVelocity, itemId, isHq);

            return await SaveToDatabaseAsync(itemId, isHq, minListing, averageSalePrice, recentPurchase, dailySaleVelocity);
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

        var tasks = new List<Task>();

        if (minListing is not null)
        {
            minListingDb = new MinListingPoco(0,
                itemId,
                isHq,
                minListing.WorldDataPointId,
                minListing.DcDataPointId,
                minListing.RegionDataPointId);
            tasks.Add(dbContext.MinListing.AddAsync(minListingDb).AsTask());
        }

        if (averageSalePrice is not null)
        {
            averageSalePriceDb = new AverageSalePricePoco(0,
                itemId,
                isHq,
                averageSalePrice.WorldDataPointId,
                averageSalePrice.DcDataPointId,
                averageSalePrice.RegionDataPointId);
            tasks.Add(dbContext.AverageSalePrice.AddAsync(averageSalePriceDb).AsTask());
        }

        if (recentPurchase is not null)
        {
            recentPurchaseDb = new RecentPurchasePoco(0,
                itemId,
                isHq,
                recentPurchase.WorldDataPointId,
                recentPurchase.DcDataPointId,
                recentPurchase.RegionDataPointId);
            tasks.Add(dbContext.RecentPurchase.AddAsync(recentPurchaseDb).AsTask());
        }

        if (dailySaleVelocity is not null)
        {
            dailySaleVelocityDb = new DailySaleVelocityPoco(0,
                itemId,
                isHq,
                dailySaleVelocity.WorldQuantity,
                dailySaleVelocity.DcQuantity,
                dailySaleVelocity.RegionQuantity);
            tasks.Add(dbContext.DailySaleVelocity.AddAsync(dailySaleVelocityDb).AsTask());
        }

        await Task.WhenAll(tasks);
        await dbContext.SaveChangesAsync();

        return new QualityPriceDataPoco(minListingDb, averageSalePriceDb, recentPurchaseDb, dailySaleVelocityDb);
    }
}