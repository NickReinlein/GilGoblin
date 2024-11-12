using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IQualityPriceDataConverter
{
    Task<QualityPriceDataPoco?> ConvertAndSaveDetailsAsync(
        QualityPriceDataWebPoco? qualityData,
        int worldId,
        int itemId,
        bool isHq);
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
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.MinListing, itemId, worldId, isHq))
                .AsMinListingPoco();
            var averageSalePrice =
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.AverageSalePrice, itemId, worldId, isHq))
                .AsAverageSalePricePoco();
            var recentPurchase =
                (await dataPointConverter.ConvertAndSaveAsync(qualityData.RecentPurchase, itemId, worldId, isHq))
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
        await SaveToDatabase(averageSalePrice);
        await SaveToDatabase(minListing);
        await SaveToDatabase(recentPurchase);
        await SaveToDatabase(dailySaleVelocity);

        return new QualityPriceDataPoco(averageSalePrice, minListing, recentPurchase, dailySaleVelocity);
    }

    private async Task SaveToDatabase<T>(T? poco)
        where T : IdentifiableTripleKeyPoco
    {
        if (poco is null)
            return;

        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var existing = await dbContext.Set<T>()
            .FirstOrDefaultAsync(e =>
                e.ItemId == poco.ItemId &&
                e.WorldId == poco.WorldId &&
                e.IsHq == poco.IsHq);

        if (existing is not null) return;

        dbContext.Add(poco);
        await dbContext.SaveChangesAsync();
    }
}