using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public interface IDataSaver<in T> where T : class
{
    Task<bool> SaveAsync(IEnumerable<T> updates, CancellationToken ct = default);
}

public class DataSaver<T>(IServiceProvider serviceProvider, ILogger<DataSaver<T>> logger) : IDataSaver<T>
    where T : class, IIdentifiable
{
    protected readonly IServiceProvider ServiceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private readonly ILogger<DataSaver<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<bool> SaveAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        var entityList = entities.ToList();
        if (!entityList.Any())
            return false;

        try
        {
            var filteredUpdates = FilterInvalidEntities(entityList);
            if (filteredUpdates.Count == 0)
                throw new ArgumentException("No valid entities remained after validity check");

            var savedCount = await UpdateContextAsync(filteredUpdates, ct);
            _logger.LogInformation($"Saved {savedCount} new entries for type {typeof(T).Name}");

            var failedCount = entityList.Count - savedCount;
            if (failedCount == 0)
                return true;

            _logger.LogError(
                $"Failed to save {failedCount} entities, out of {entityList.Count} total entities");
            throw new DbUpdateException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update due to error");
            return false;
        }
    }

    protected virtual async Task<int> UpdateContextAsync(List<T> entityList, CancellationToken ct = default)
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
            _logger.LogError(ex, "Failed to update due to error");
            return 0;
        }
    }

    protected virtual List<T> FilterInvalidEntities(IEnumerable<T> entities)
    {
        return entities.ToList();
    }
    
    protected virtual BulkConfig GetBulkConfig()
    {
        return new BulkConfig
        {
            UpdateByProperties = ["Id"], // unique keys
            PreserveInsertOrder = false,
            SetOutputIdentity = true
        };
    }
}