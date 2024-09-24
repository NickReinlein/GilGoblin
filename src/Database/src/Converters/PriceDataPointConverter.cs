using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IPriceDataPointConverter
{
    Task<PriceDataPointPoco?> ConvertAsync(PriceDataPointWebPoco? dataPoint, int itemId, bool isHq);
}

public class PriceDataPointConverter(
    IServiceProvider serviceProvider,
    IPriceDataDetailConverter detailConverter,
    ILogger<PriceDataPointConverter> logger)
    : IPriceDataPointConverter
{
    public async Task<PriceDataPointPoco?> ConvertAsync(PriceDataPointWebPoco? dataPoint, int itemId, bool isHq)
    {
        try
        {
            if (dataPoint is null || itemId < 1)
                throw new ArgumentException("Invalid item id or datapoint");
            
            if (!dataPoint.HasValidPrice())
                throw new DataException("Invalid price data");

            var world = detailConverter.Convert(dataPoint.World, "World");
            var dc = detailConverter.Convert(dataPoint.Dc, "Dc");
            var region = detailConverter.Convert(dataPoint.Region, "Region");

            var pricePoints =
                new List<PriceDataPoco?> { world, dc, region }
                    .Where(x => x is not null)
                    .Cast<PriceDataPoco>()
                    .ToList();
            if (!pricePoints.Any())
                return null;

            await using var scope = serviceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            await dbContext.PriceData.AddRangeAsync(pricePoints);
            var savedCount = await dbContext.SaveChangesAsync();
            if (savedCount < pricePoints.Count)
                logger.LogWarning(
                    $"Failed to save {pricePoints.Count - savedCount} of {pricePoints.Count} price data points");

            return new PriceDataPointPoco(0, itemId, isHq, world?.Id, dc?.Id, region?.Id);
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert price data");
            return null;
        }
    }
}