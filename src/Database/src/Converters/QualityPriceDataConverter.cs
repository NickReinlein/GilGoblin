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
    Task<QualityPriceDataPoco?> ConvertAndSaveDetailsAsync(QualityPriceDataWebPoco? qualityData, int worldId,
        int itemId, bool isHq);
}

public class QualityPriceDataConverter(
    IServiceProvider serviceProvider,
    IPriceDataPointConverter dataPointConverter,
    IDailySaleVelocityConverter saleVelocityConverter,
    ILogger<QualityPriceDataConverter> logger)
    : IQualityPriceDataConverter
{
    public async Task<QualityPriceDataPoco?> ConvertAndSaveDetailsAsync(
        QualityPriceDataWebPoco? qualityData,
        int worldId,
        int itemId,
        bool isHq)
    {
        try
        {
            if (qualityData is null || itemId < 1 || !qualityData.HasValidPrice())
                return null;

            var minListing =
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.MinListing, itemId, isHq))
                .AsMinListingPoco();
            var averageSalePrice =
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.AverageSalePrice, itemId, isHq))
                .AsAverageSalePricePoco();
            var recentPurchase =
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.RecentPurchase, itemId, isHq))
                .AsRecentPurchasePoco();
            var dailySaleVelocity =
                await saleVelocityConverter.ConvertAndSaveAsync(qualityData.DailySaleVelocity, itemId, worldId, isHq);

            return await ConvertToQualityPriceDataAndSave(minListing, averageSalePrice,
                recentPurchase, dailySaleVelocity);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }

    private async Task<QualityPriceDataPoco> ConvertToQualityPriceDataAndSave(
        MinListingPoco? minListing,
        AverageSalePricePoco? averageSalePrice,
        RecentPurchasePoco? recentPurchase,
        DailySaleVelocityPoco? dailySaleVelocity)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

        if (minListing is not null)
            dbContext.MinListing.Add(minListing);

        if (averageSalePrice is not null)
            dbContext.AverageSalePrice.Add(averageSalePrice);

        if (recentPurchase is not null)
            dbContext.RecentPurchase.Add(recentPurchase);

        if (dailySaleVelocity is not null)
            dbContext.DailySaleVelocity.Add(dailySaleVelocity);

        var saved = await dbContext.SaveChangesAsync();
        if (saved < 1)
            throw new DataException("Failed to save quality price data");

        var qualityPriceDataPoco =
            new QualityPriceDataPoco(minListing, averageSalePrice, recentPurchase, dailySaleVelocity);
        return qualityPriceDataPoco;
    }
}