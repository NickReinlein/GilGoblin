using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataDetailConverter
{
    Task<PriceDataPoco?> ConvertAsync(PriceDataDetailPoco? dataPoint, string priceType);
}

public class PriceDataDetailConverter(IServiceProvider serviceProvider, ILogger<PriceDataDetailConverter> logger)
    : IPriceDataDetailConverter
{
    public async Task<PriceDataPoco?> ConvertAsync(PriceDataDetailPoco? dataPoint, string priceType)
    {
        try
        {
            if (dataPoint is null || string.IsNullOrEmpty(priceType) || !dataPoint.HasValidPrice())
                throw new ArgumentException("Invalid price data", nameof(dataPoint));

            var pricePoint = new PriceDataPoco(0, priceType, dataPoint.Price, dataPoint.WorldId, dataPoint.Timestamp);
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            await dbContext.PriceData.AddAsync(pricePoint);
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