using System.Collections;
using System.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GilGoblin.Web;
using GilGoblin.Services;
using GilGoblin.Database;

namespace GilGoblin.Services.DataUpdater;

public interface IDataUpdater<T>
    where T : class
{
    Task UpdateAsync();
    Task SaveUpdatedAsync(IEnumerable<T> updated);
}

public abstract class DataUpdater<T, U> : IDataUpdater<T>
    where T : class
    where U : class, IReponseToList<T>
{
    private readonly IDataFetcher<T, U> _fetcher;
    private readonly GilGoblinDbContext _dbContext;
    private readonly ILogger<DataUpdater<T, U>> _logger;
    private readonly Timer timer;

    public DataUpdater(
        GilGoblinDbContext dbContext,
        IDataFetcher<T, U> fetcher,
        ILogger<DataUpdater<T, U>> logger
    )
    {
        _dbContext = dbContext;
        _fetcher = fetcher;
        _logger = logger;
        timer = new Timer(
            async _ => await UpdateAsync(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(5)
        );
    }

    public async Task UpdateAsync()
    {
        try
        {
            var entriesToUpdate = await GetEntriesToUpdateAsync();
            if (!entriesToUpdate.Any())
                return;

            var result = await FetchUpdateForEntries(entriesToUpdate);

            await SaveUpdatedAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occured during the Api call: {ex.Message}");
        }
    }

    public async Task SaveUpdatedAsync(IEnumerable<T> updated)
    {
        if (!updated.Any())
            return;

        using var context = _dbContext; // todo check this
        await context.AddRangeAsync(updated);
        await context.SaveChangesAsync();
        _logger.LogInformation($"Saved {updated.Count()} entries for type {typeof(T).Name}");
    }

    protected virtual async Task<IEnumerable<T>> FetchUpdateForEntries(
        IEnumerable<T> entriesToUpdate
    )
    {
        var apiUrl = await GetUrlPathFromEntries(entriesToUpdate);

        var response = await _fetcher.GetMultipleAsync(apiUrl);
        if (response is null)
            throw new HttpRequestException($"Failed to fetch the apiUrl: {apiUrl}");

        return response.GetContentAsList();
    }

    protected virtual async Task<IEnumerable<T>> GetEntriesToUpdateAsync() => Enumerable.Empty<T>();

    protected virtual async Task<string> GetUrlPathFromEntries(IEnumerable<T> entries) =>
        string.Empty;
}
