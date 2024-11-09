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
    Task<DailySaleVelocityPoco?> ConvertAndSaveAsync(
        DailySaleVelocityWebPoco? saleVelocity,
        int itemId,
        int worldId,
        bool isHq,
        CancellationToken ct = default);
}

public class DailySaleVelocityConverter(
    IDataSaver<DailySaleVelocityPoco> saver,
    ILogger<PriceDataPointConverter> logger)
    : IDailySaleVelocityConverter
{
    public async Task<DailySaleVelocityPoco?> ConvertAndSaveAsync(DailySaleVelocityWebPoco? saleVelocity, int itemId,
        int worldId,
        bool isHq, CancellationToken ct = default)
    {
        try
        {
            if (saleVelocity is null || itemId < 1)
                throw new ArgumentException("Invalid sale velocity: null or invalid item id", nameof(saleVelocity));
            if (!saleVelocity.HasAValidQuantity())
                throw new DataException("Has no valid quantity");

            var dailySaleVelocityDb = ConvertToDbFormat(saleVelocity, itemId, worldId, isHq);

            await saver.SaveAsync([dailySaleVelocityDb], ct);
            return dailySaleVelocityDb;
        }
        catch (Exception e)
        {
            logger.LogDebug(e, "Failed to convert daily sale velocity");
            return null;
        }
    }

    private static DailySaleVelocityPoco ConvertToDbFormat(
        DailySaleVelocityWebPoco? saleVelocity,
        int itemId,
        int worldId,
        bool isHq)
        => new(itemId,
            worldId,
            isHq,
            saleVelocity?.World?.Quantity,
            saleVelocity?.Dc?.Quantity,
            saleVelocity?.Region?.Quantity);
}