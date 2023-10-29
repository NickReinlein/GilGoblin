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
            var newEntries = await UpdatedExistingEntries(updatesList);
            await DbContext.AddRangeAsync(newEntries);

            await DbContext.SaveChangesAsync();
            _logger.LogInformation($"Saved {updatesList.Count} entries for type {typeof(T).Name}");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to updated due to error: {e.Message}");
            return false;
        }
    }

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