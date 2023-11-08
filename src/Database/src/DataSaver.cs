using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class DataSaver<T> : IDataSaver<T> where T : class, IIdentifiable
{
    protected readonly GilGoblinDbContext DbContext;
    private readonly ILogger<DataSaver<T>> _logger;

    public DataSaver(GilGoblinDbContext dbContext, ILogger<DataSaver<T>> logger)
    {
        DbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> SaveAsync(IEnumerable<T> updates)
    {
        var updatesList = updates.ToList();
        if (!updatesList.Any())
            return false;

        try
        {
            var success = SanityCheck(updatesList);
            if (!success)
                throw new ArgumentException("Cannot save price due to error in key field");

            var newEntries = await UpdatedExistingEntries(updatesList);
            await DbContext.AddRangeAsync(newEntries);

            var savedCount = await DbContext.SaveChangesAsync();
            _logger.LogInformation($"Saved {savedCount} entries for type {typeof(T).Name}");

            var failedCount = updatesList.Count - savedCount;
            if (failedCount > 0)
                throw new Exception($"Failed to save {failedCount} entities!");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to update due to error: {e.Message}");
            return false;
        }
    }

    public virtual bool SanityCheck(IEnumerable<T> updates)
        => !updates.Any(t => t.GetId() < 0);

    protected async Task<List<T>> UpdatedExistingEntries(List<T> updatedEntries)
    {
        var newEntriesList = new List<T>();
        foreach (var updated in updatedEntries)
        {
            if (await ShouldBeUpdated(updated))
                DbContext.Entry(updated).CurrentValues.SetValues(updated);
            else
                newEntriesList.Add(updated);
        }

        return newEntriesList;
    }

    protected virtual async Task<bool> ShouldBeUpdated(T updated)
        => updated.GetId() > 0 &&
           await DbContext.Set<T>().FindAsync(updated.GetId()) != null;
}