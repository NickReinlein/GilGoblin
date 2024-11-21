using System;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataDetailConverter
{
    PriceDataPoco? Convert(PriceDataDetailPoco? dataPoint, string priceType, int worldId);
}

public class PriceDataDetailConverter(ILogger<PriceDataDetailConverter> logger)
    : IPriceDataDetailConverter
{
    public PriceDataPoco? Convert(PriceDataDetailPoco? dataPoint, string priceType, int worldId)
    {
        try
        {
            if (dataPoint is null || string.IsNullOrEmpty(priceType) || !dataPoint.HasValidPrice())
                throw new ArgumentException("Invalid price data", nameof(dataPoint));

            return new PriceDataPoco(priceType, dataPoint.Price, worldId, dataPoint.Timestamp ?? DateTime.UtcNow.Ticks);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}