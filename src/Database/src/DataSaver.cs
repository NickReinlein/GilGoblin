using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class DataSaver<T> : IDataSaver<T> where T : class, IIdentifiable
{
    protected readonly GilGoblinDbContext _context;
    private readonly ILogger<DataSaver<T>> _logger;

    public DataSaver(GilGoblinDbContext context, ILogger<DataSaver<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SaveAsync(IEnumerable<T> updates)
    {
        var updateList = updates.ToList();
        if (!updateList.Any())
            return false;

        try
        {
            var success = SanityCheck(updateList);
            if (!success)
                throw new ArgumentException("Cannot save price due to error in key field");

             foreach (var updated in updateList)
            {
                _context.Entry(updated).State = updated.GetId() == 0 ? EntityState.Added : EntityState.Modified;
            }

            var savedCount = await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved {savedCount} new entries for type {typeof(T).Name}");

            var failedCount = updateList.Count - savedCount;
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

    public virtual bool SanityCheck(IEnumerable<T> updates) => !updates.Any(t => t.GetId() < 0);
}