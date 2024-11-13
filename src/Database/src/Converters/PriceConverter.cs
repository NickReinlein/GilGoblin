using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceConverter
{
    Task<(PricePoco?, PricePoco?)> ConvertAndSaveAsync(
        PriceWebPoco webPoco,
        int worldId,
        CancellationToken ct = default);
}

public class PriceConverter(
    IQualityPriceDataConverter qualityConverter,
    IPriceSaver priceSaver,
    ILogger<PriceConverter> logger) : IPriceConverter
{
    public async Task<(PricePoco?, PricePoco?)> ConvertAndSaveAsync(
        PriceWebPoco webPoco,
        int worldId,
        CancellationToken ct = default)
    {
        try
        {
            if (webPoco is null || worldId < 1)
                throw new ArgumentException("Invalid world id", nameof(worldId));

            var itemId = webPoco.ItemId;
            var nqPrices = await qualityConverter.ConvertAndSaveDetailsAsync(webPoco.Nq, worldId, itemId, false);
            var nq = nqPrices is null ? null : GetPricePocoFromQualityPrices(nqPrices, worldId, itemId, false);
            var hqPrices = await qualityConverter.ConvertAndSaveDetailsAsync(webPoco.Hq, worldId, itemId, true);
            var hq = hqPrices is null ? null : GetPricePocoFromQualityPrices(hqPrices, worldId, itemId, true);

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

    private static PricePoco? GetPricePocoFromQualityPrices(QualityPriceDataPoco? quality, int worldId, int itemId,
        bool isHq)
    {
        return quality is null
            ? null
            : new PricePoco(
                ItemId: itemId,
                WorldId: worldId,
                IsHq: isHq,
                AverageSalePriceId: quality.AverageSalePrice?.Id,
                MinListingId: quality.MinListing?.Id,
                RecentPurchaseId: quality.RecentPurchase?.Id,
                DailySaleVelocityId: quality.DailySaleVelocity?.Id
            );
    }
}