using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IDailySaleVelocityConverter
{
    Task<DailySaleVelocityPoco?> ConvertAsync(DailySaleVelocityWebPoco? saleVelocity, int itemId, bool isHq);
}

public class DailySaleVelocityConverter(
    IServiceProvider serviceProvider,
    ILogger<PriceDataPointConverter> logger)
    : IDailySaleVelocityConverter
{
    public async Task<DailySaleVelocityPoco?> ConvertAsync(DailySaleVelocityWebPoco? saleVelocity, int itemId,
        bool isHq)
    {
        try
        {
            if (saleVelocity is null || itemId < 1 || !saleVelocity.HasAValidQuantity())
                throw new ArgumentException("Invalid sale velocity");

            var dailySaleVelocityDb = ConvertToDbFormat(saleVelocity, itemId, isHq);

            await SaveToDatabaseAsync(dailySaleVelocityDb);
            return dailySaleVelocityDb;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert daily sale velocity");
            return null;
        }
    }

    private async Task SaveToDatabaseAsync(DailySaleVelocityPoco dailySaleVelocityDb)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        await dbContext.DailySaleVelocity.AddRangeAsync(dailySaleVelocityDb);
        await dbContext.SaveChangesAsync();
    }

    private static DailySaleVelocityPoco ConvertToDbFormat(DailySaleVelocityWebPoco? saleVelocity, int itemId, bool isHq)
    {
        return new DailySaleVelocityPoco(0, 
            itemId, 
            isHq,
            saleVelocity?.WorldQuantity, 
            saleVelocity?.DcQuantity,
            saleVelocity?.RegionQuantity);
    }
}