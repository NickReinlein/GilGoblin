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

        var newEntries = updatesList.Where(i => i.GetId() == 0).ToList();
        await DbContext.AddRangeAsync(newEntries);

        var updatedEntries = updatesList.Except(newEntries).ToList();
        await UpdatedExistingEntries(updatedEntries);

        await DbContext.SaveChangesAsync();
        _logger.LogInformation($"Saved {updatesList.Count} entries for type {typeof(T).Name}");
        return true;
    }

    protected virtual async Task UpdatedExistingEntries(List<T> updatedEntries)
    {
        foreach (var updated in updatedEntries)
        {
            var existingEntity = await DbContext.Set<T>().FindAsync(updated.GetId());
            if (existingEntity != null)
                DbContext.Entry(existingEntity).CurrentValues.SetValues(updated);
        }
    }
}