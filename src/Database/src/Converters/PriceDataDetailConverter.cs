using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataDetailConverter
{
    PriceDataPoco? Convert(PriceDataDetailPoco? dataPoint, string priceType);
}

public class PriceDataDetailConverter(ILogger<PriceDataDetailConverter> logger)
    : IPriceDataDetailConverter
{
    public PriceDataPoco? Convert(PriceDataDetailPoco? dataPoint, string priceType)
    {
        try
        {
            if (dataPoint is null || string.IsNullOrEmpty(priceType) || !dataPoint.HasValidPrice())
                throw new ArgumentException("Invalid price data", nameof(dataPoint));

            return new PriceDataPoco(priceType, dataPoint.Price, dataPoint.WorldId, dataPoint.Timestamp);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}