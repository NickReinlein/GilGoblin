using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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

            var savedCount = await UpdateContextAsync(filteredUpdates);
            _logger.LogInformation($"Saved {savedCount} new entries for type {typeof(T).Name}");

            var failedCount = entityList.Count - savedCount;
            if (failedCount == 0)
                return true;

            _logger.LogError(
                $"Failed to save {failedCount} entities, out of {entityList.Count} total entities");
            throw new DbUpdateException();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update due to database error");
            return false;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Failed to update due to invalid data");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update due to unknown error");
            return false;
        }
    }

    protected virtual async Task<int> UpdateContextAsync(List<T> entityList)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        foreach (var entity in entityList)
        {
            dbContext.Entry(entity).State = entity.GetId() == 0 ? EntityState.Added : EntityState.Modified;
        }

        return await dbContext.SaveChangesAsync();
    }

    protected virtual List<T> FilterInvalidEntities(IEnumerable<T> entities)
    {
        return entities.ToList();
    }
}