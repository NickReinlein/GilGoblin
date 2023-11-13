using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using GilGoblin.Database;
using Microsoft.Extensions.Logging;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.Accountant;

public interface IAccountant<T> where T : class, IIdentifiable
{
    Task CalculateAsync(CancellationToken ct, int worldId);
}

public class Accountant<T> : BackgroundService, IAccountant<T>
    where T : class, IIdentifiable
{
    protected readonly IServiceScopeFactory ScopeFactory;
    protected readonly ILogger<Accountant<T>> Logger;

    public Accountant(
        IServiceScopeFactory scopeFactory,
        ILogger<Accountant<T>> logger)
    {
        ScopeFactory = scopeFactory;
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var worldIds = GetWorldIds();
                if (!worldIds.Any())
                    throw new DataException("Failed to find any world Ids. This should never happen");

                foreach (var worldId in worldIds)
                {
                    Logger.LogInformation($"Opening ledger to update {typeof(T)} for world {worldId}");
                    await CalculateAsync(ct, worldId);
                }
            }
            catch (DataException ex)
            {
                Logger.LogCritical($"A critical error occured: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError($"An unexpected exception occured during accounting process: {ex.Message}");
            }

            var delay = TimeSpan.FromMinutes(5);
            Logger.LogInformation($"Awaiting delay of {delay}");
            await Task.Delay(delay, ct);
        }
    }

    public async Task CalculateAsync(CancellationToken ct, int worldId)
    {
        var idList = GetIdsToUpdate(worldId);
        if (!idList.Any())
        {
            Logger.LogInformation($"Nothing to calculate for {worldId}");
            return;
        }

        if (!ct.IsCancellationRequested)
            await ComputeAsync(worldId, idList, ct);
    }

    protected virtual async Task ComputeAsync(int worldId, List<int> idList, CancellationToken ct)
        => throw new NotImplementedException();

    protected virtual List<int> GetWorldIds() => new() { 34 };

    protected GilGoblinDbContext GetDbContext()
    {
        using var scope = ScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
    }

    protected virtual List<int> GetIdsToUpdate(int worldId) => new();
}