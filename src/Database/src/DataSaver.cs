using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
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

        await using var context = DbContext;
        foreach (var update in updatesList)
        {
            context.Entry(update).State =
                update.GetId() > 0
                    ? EntityState.Added
                    : EntityState.Modified;
        }
        await context.SaveChangesAsync();
        _logger.LogInformation($"Saved {updatesList.Count()} entries for type {typeof(T).Name}");
        return true;
    }
}