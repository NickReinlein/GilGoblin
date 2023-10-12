using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Services;

public class DataSaver<T> : IDataSaver<T> where T : class
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
        await context.AddRangeAsync(updatesList);
        await context.SaveChangesAsync();
        _logger.LogInformation($"Saved {updatesList.Count()} entries for type {typeof(T).Name}");
        return true;
    }
}