using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataDetailConverter
{
    Task<PriceDataPoco?> ConvertAsync(PriceDataDetailPoco? dataPoint, string priceType);
}

public class PriceDataDetailConverter(GilGoblinDbContext dbContext, ILogger<PriceDataDetailConverter> logger) : IPriceDataDetailConverter
{
    public async Task<PriceDataPoco?> ConvertAsync(PriceDataDetailPoco? dataPoint, string priceType)
    {
        try
        {
            if (dataPoint is null || string.IsNullOrEmpty(priceType) || !dataPoint.HasValidPrice())
                throw new ArgumentException("Invalid price data", nameof(dataPoint));

            var pricePoint = new PriceDataPoco(0, priceType, dataPoint.Price, dataPoint.WorldId, dataPoint.Timestamp);
            dbContext.PriceData.Add(pricePoint);
            await dbContext.SaveChangesAsync();
            return pricePoint;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}