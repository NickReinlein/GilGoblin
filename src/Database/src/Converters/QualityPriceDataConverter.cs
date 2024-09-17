using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IQualityPriceDataConverter
{
    Task<QualityPriceDataPoco?> ConvertAsync(QualityPriceDataWebPoco? qualityData, int itemId, bool isHq);
}

public class QualityPriceDataConverter(
    GilGoblinDbContext dbContext,
    IPriceDataPointConverter dataPointConverter,
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

            var minListingDb = minListing is null
                ? null
                : new MinListingPoco(0, itemId, isHq, minListing.WorldDataPointId,
                    minListing.DcDataPointId, minListing.RegionDataPointId);
            if (minListingDb is not null)
                dbContext.MinListing.Add(minListingDb);

            var averageSalePriceDb = averageSalePrice is null
                ? null
                : new AverageSalePricePoco(0, itemId, isHq, averageSalePrice.WorldDataPointId,
                    averageSalePrice.DcDataPointId, averageSalePrice.RegionDataPointId);
            if (averageSalePriceDb is not null)
                dbContext.AverageSalePrice.Add(averageSalePriceDb);

            var recentPurchaseDb = recentPurchase is null
                ? null
                : new RecentPurchasePoco(0, itemId, isHq, recentPurchase.WorldDataPointId,
                    recentPurchase.DcDataPointId, recentPurchase.RegionDataPointId);
            if (recentPurchaseDb is not null)
                dbContext.RecentPurchase.Add(recentPurchaseDb);
            
            await dbContext.SaveChangesAsync();

            var converted = new QualityPriceDataPoco(minListingDb, averageSalePriceDb, recentPurchaseDb, null);
            return converted;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}