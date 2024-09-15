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
    IPriceDataPointConverter dataPointConverter,
    ILogger<QualityPriceDataConverter> logger)
    : IQualityPriceDataConverter
{
    public async Task<QualityPriceDataPoco?> ConvertAsync(QualityPriceDataWebPoco? qualityData, int itemId, bool isHq)
    {
        try
        {
            if (qualityData is null || itemId < 1 || !qualityData.HasValidPrice())
                throw new ArgumentException("Invalid price data", nameof(qualityData));

            var minListing = await dataPointConverter.ConvertAsync(qualityData.MinListing, itemId, isHq);
            var averageSalePrice = await dataPointConverter.ConvertAsync(qualityData.AverageSalePrice, itemId, isHq);
            var recentPurchase = await dataPointConverter.ConvertAsync(qualityData.RecentPurchase, itemId, isHq);
            var converted = new QualityPriceDataPoco(minListing, averageSalePrice, recentPurchase, null);
            return converted;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}