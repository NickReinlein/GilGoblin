using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using GilGoblin.Api.Crafting;
using Microsoft.Extensions.Logging;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.Accountant;

public interface IAccountant<T> where T : class, IIdentifiable
{
    Task CalculateAsync(CancellationToken ct, int worldId);
    Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct);
    Task<T?> ComputeAsync(int worldId, int recipeId, ICraftingCalculator calc);
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

        if (ct.IsCancellationRequested)
        {
            Logger.LogInformation($"Cancellation of the task by the user. Putting away the books for {worldId}");
            return;
        }

        try
        {
            await ComputeListAsync(worldId, idList, ct);
            Logger.LogInformation($"Boss, books are closed for world {worldId}");
        }
        catch (Exception e)
        {
            var message = $"Failed to balance the books for world {worldId}: {e.Message}";
            Logger.LogError(message);
        }
    }

    public virtual Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
        => throw new NotImplementedException();

    public virtual Task<T?> ComputeAsync(int worldId, int idList, ICraftingCalculator calc)
        => throw new NotImplementedException();

    public virtual List<int> GetWorldIds() => new() { 34 };

    public virtual List<int> GetIdsToUpdate(int worldId) => new();
}