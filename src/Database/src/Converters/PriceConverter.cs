using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId);
}

public class PriceConverter(
    IServiceProvider serviceProvider,
    IQualityPriceDataConverter qualityConverter,
    ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId)
    {
        try
        {
            if (webPoco is null || worldId < 1)
                throw new ArgumentException("Invalid world id", nameof(worldId));

            var itemId = webPoco.ItemId;
            var nqPrices = await qualityConverter.ConvertAsync(webPoco.Nq, itemId, false);
            var nq = GetPricePocoFromQualityPrices(worldId, nqPrices, itemId);
            var hqPrices = await qualityConverter.ConvertAsync(webPoco.Hq, itemId, false);
            var hq = GetPricePocoFromQualityPrices(worldId, hqPrices, itemId);
            
            await using var scope = serviceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            if (hq is not null)
                dbContext.Price.Add(hq);

            if (nq is not null)
                dbContext.Price.Add(nq);

            var saved = await dbContext.SaveChangesAsync();
            logger.LogDebug("Saved {Saved} prices", saved);
            
            return (hq, nq);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return (null, null);
        }
    }

    private static PricePoco? GetPricePocoFromQualityPrices(int worldId, QualityPriceDataPoco? quality, int itemId)
    {
        return quality is null
            ? null
            : new PricePoco
            {
                ItemId = itemId,
                WorldId = worldId,
                IsHq = false,
                Updated = DateTimeOffset.UtcNow,
                AverageSalePriceId = quality?.AverageSalePrice?.Id ?? 0,
                MinListingId = quality?.MinListing?.Id ?? 0,
                RecentPurchaseId = quality?.RecentPurchase?.Id ?? 0,
                DailySaleVelocityId = quality?.DailySaleVelocity?.Id ?? 0,
            };
    }
}