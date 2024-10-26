using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Converters;

public interface IDailySaleVelocityConverter
{
    Task<DailySaleVelocityPoco?> ConvertAsync(
        DailySaleVelocityWebPoco? saleVelocity,
        int itemId,
        bool isHq,
        CancellationToken ct = default);
}

public class DailySaleVelocityConverter(
    IServiceProvider serviceProvider,
    IDataSaver<DailySaleVelocityPoco> saver,
    ILogger<PriceDataPointConverter> logger)
    : IDailySaleVelocityConverter
{
    public async Task<DailySaleVelocityPoco?> ConvertAsync(DailySaleVelocityWebPoco? saleVelocity, int itemId,
        bool isHq, CancellationToken ct = default)
    {
        try
        {
            if (saleVelocity is null || itemId < 1)
                throw new ArgumentException("Invalid sale velocity: null or invalid item id", nameof(saleVelocity));
            if (!saleVelocity.HasAValidQuantity())
                throw new DataException("Has no valid quantity");

            var dailySaleVelocityDb = ConvertToDbFormat(saleVelocity, itemId, isHq);

            // await saver.SaveAsync([dailySaleVelocityDb], ct);
            return dailySaleVelocityDb;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert daily sale velocity");
            return null;
        }
    }

    // private async Task SaveToDatabaseAsync(DailySaleVelocityPoco dailySaleVelocityDb)
    // {
    //     await using var scope = serviceProvider.CreateAsyncScope();
    //     await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
    //     await dbContext.DailySaleVelocity.AddRangeAsync(dailySaleVelocityDb);
    //     await dbContext.SaveChangesAsync();
    // }

    private static DailySaleVelocityPoco ConvertToDbFormat(
        DailySaleVelocityWebPoco? saleVelocity,
        int itemId,
        bool isHq)
        => new(itemId,
            isHq,
            saleVelocity?.World,
            saleVelocity?.Dc,
            saleVelocity?.Region);
}