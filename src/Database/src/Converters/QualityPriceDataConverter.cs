using System;
using System.Data;
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

            var qualityPriceData = await ConvertToQualityPriceData(itemId, isHq, minListing, averageSalePrice, recentPurchase,
                dailySaleVelocity);
            return qualityPriceData;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }

    private async Task<QualityPriceDataPoco> ConvertToQualityPriceData(
        int itemId,
        bool isHq,
        PriceDataPointPoco? minListing,
        PriceDataPointPoco? averageSalePrice,
        PriceDataPointPoco? recentPurchase,
        DailySaleVelocityPoco? dailySaleVelocity)
    {
        // await using var scope = serviceProvider.CreateAsyncScope();
        // await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

        MinListingPoco? minListingDb = null;
        AverageSalePricePoco? averageSalePriceDb = null;
        RecentPurchasePoco? recentPurchaseDb = null;

        if (minListing is not null)
        {
            minListingDb = new MinListingPoco(
                itemId,
                isHq,
                minListing.WorldDataPointId,
                minListing.DcDataPointId,
                minListing.RegionDataPointId);
            // dbContext.MinListing.Add(minListingDb);
        }

        if (averageSalePrice is not null)
        {
            averageSalePriceDb = new AverageSalePricePoco(
                itemId,
                isHq,
                averageSalePrice.WorldDataPointId,
                averageSalePrice.DcDataPointId,
                averageSalePrice.RegionDataPointId);
            // dbContext.AverageSalePrice.Add(averageSalePriceDb);
        }

        if (recentPurchase is not null)
        {
            recentPurchaseDb = new RecentPurchasePoco(
                itemId,
                isHq,
                recentPurchase.WorldDataPointId,
                recentPurchase.DcDataPointId,
                recentPurchase.RegionDataPointId);
            // dbContext.RecentPurchase.Add(recentPurchaseDb);
        }

        // if (dailySaleVelocity is not null)
        // {
        //     dbContext.DailySaleVelocity.Add(dailySaleVelocity);
        // }
        //
        // var saved = await dbContext.SaveChangesAsync();
        // if (saved < 1)
        //     throw new DataException("Failed to save quality price data");

        var qualityPriceDataPoco =
            new QualityPriceDataPoco(minListingDb, averageSalePriceDb, recentPurchaseDb, dailySaleVelocity);
        return qualityPriceDataPoco;
    }
}