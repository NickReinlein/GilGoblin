using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId, CancellationToken ct = default);
}

public class PriceConverter(
    IServiceProvider serviceProvider,
    IQualityPriceDataConverter qualityConverter,
    IPriceSaver priceSaver,
    ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?, PricePoco?)> ConvertAsync(PriceWebPoco webPoco, int worldId,
        CancellationToken ct = default)
    {
        try
        {
            if (webPoco is null || worldId < 1)
                throw new ArgumentException("Invalid world id", nameof(worldId));

            var itemId = webPoco.ItemId;
            var nqPrices = await qualityConverter.ConvertAsync(webPoco.Nq, itemId, false);
            var nq = GetPricePocoFromQualityPrices(worldId, nqPrices, itemId);
            var hqPrices = await qualityConverter.ConvertAsync(webPoco.Hq, itemId, true);
            var hq = GetPricePocoFromQualityPrices(worldId, hqPrices, itemId);

            await SaveToDatabaseAsync(hq, nq, ct);
            return (hq, nq);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return (null, null);
        }
    }

    private async Task SaveToDatabaseAsync(PricePoco? hq, PricePoco? nq, CancellationToken ct)
    {
        if (hq is null && nq is null)
            return;

        try
        {
            var saveList = new List<PricePoco?> { hq, nq }
                .Where(x => x is not null)
                .Cast<PricePoco>()
                .ToList();

            var success = await priceSaver.SaveAsync(saveList, ct);
            if (success)
                logger.LogDebug("Saved {Saved} prices", saveList.Count);
            else
                throw new DataException();
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to save price data");
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