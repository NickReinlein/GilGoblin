using System;
using System.Data;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.EntityFrameworkCore;
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
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

        if (minListing is not null)
        {
            await UpdateOrAddToContext(minListing, dbContext);
        }

        if (averageSalePrice is not null)
        {
            await UpdateOrAddToContext(averageSalePrice, dbContext);
        }

        if (recentPurchase is not null)
        {
            await UpdateOrAddToContext(recentPurchase, dbContext);
        }

        if (dailySaleVelocity is not null)
            await UpdateOrAddToContext(dailySaleVelocity, dbContext);

        var saved = await dbContext.SaveChangesAsync();
        if (saved < 1)
            throw new DataException("Failed to save quality price data");

        return new QualityPriceDataPoco(minListing, averageSalePrice, recentPurchase, dailySaleVelocity);
    }

    private static async Task UpdateOrAddToContext<T>(T poco, GilGoblinDbContext dbContext)
        where T : IdentifiableTripleKeyPoco
    {
        var key = poco.GetKey();
        var existing = await dbContext.Set<T>()
            .FirstOrDefaultAsync(e =>
                e.ItemId == key.Item1 &&
                e.WorldId == key.Item2 &&
                e.IsHq == key.Item3);

        if (existing is not null)
            dbContext.Entry(existing).CurrentValues.SetValues(poco);
        else
            dbContext.Add(poco);
    }
}