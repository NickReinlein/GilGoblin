using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public interface IDataSaver<in T> where T : class
{
    Task<bool> SaveAsync(IEnumerable<T> updates, CancellationToken ct = default);
}

public class DataSaver<T>(GilGoblinDbContext context, ILogger<DataSaver<T>> logger) : IDataSaver<T>
    where T : class, IIdentifiable
{
    protected readonly GilGoblinDbContext Context = context ?? throw new ArgumentNullException(nameof(context));
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

            UpdateContext(filteredUpdates);

            var savedCount = await Context.SaveChangesAsync(ct);
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

    protected virtual void UpdateContext(List<T> entityList)
    {
        foreach (var entity in entityList)
        {
            Context.Entry(entity).State = entity.GetId() == 0 ? EntityState.Added : EntityState.Modified;
        }
    }

    protected virtual List<T> FilterInvalidEntities(IEnumerable<T> entities)
    {
        return entities.Where(t => t.GetId() >= 0).ToList();
    }
}