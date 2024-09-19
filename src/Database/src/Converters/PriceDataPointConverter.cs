using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataPointConverter
{
    Task<PriceDataPointPoco?> ConvertAsync(PriceDataPointWebPoco? dataPoint, int itemId, bool isHq);
}

public class PriceDataPointConverter(IPriceDataDetailConverter detailConverter, ILogger<PriceDataPointConverter> logger)
    : IPriceDataPointConverter
{
    public async Task<PriceDataPointPoco?> ConvertAsync(PriceDataPointWebPoco? dataPoint, int itemId, bool isHq)
    {
        try
        {
            if (dataPoint is null || itemId < 1)
                throw new ArgumentException("Invalid item id or datapoint");

            var world = await detailConverter.ConvertAsync(dataPoint.World, "World");
            var dc = await detailConverter.ConvertAsync(dataPoint.Dc, "Dc");
            var region = await detailConverter.ConvertAsync(dataPoint.Region, "Region");
            var priceDataPoint = new PriceDataPointPoco(0, itemId, isHq, world?.Id, dc?.Id, region?.Id);
            return priceDataPoint;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}