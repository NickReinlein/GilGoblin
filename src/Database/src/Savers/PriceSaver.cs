using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Database.Savers;

public interface IPriceSaver : IDataSaver<PricePoco>;

public class PriceSaver(IServiceProvider serviceProvider, ILogger<DataSaver<PricePoco>> logger)
    : DataSaver<PricePoco>(serviceProvider, logger), IPriceSaver
{
    protected override async Task<int> UpdateContextAsync(List<PricePoco> entityList, CancellationToken ct = default)
    {
        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            await dbContext.BulkInsertOrUpdateAsync(entityList, GetBulkConfig(), cancellationToken: ct);
            return entityList.Count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update due to error");
            return 0;
        }
    }
    
    protected override List<PricePoco> FilterInvalidEntities(IEnumerable<PricePoco> entities)
    {
        return entities.Where(t => t is { ItemId: > 0, WorldId : > 0 }).ToList();
    }

    protected override BulkConfig GetBulkConfig()
    {
        return new BulkConfig
        {
            UpdateByProperties = ["ItemId", "WorldId", "IsHq"], // unique keys
            PreserveInsertOrder = false,
            SetOutputIdentity = true
        };
    }
}