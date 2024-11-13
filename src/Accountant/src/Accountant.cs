using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.Accountant;

// ReSharper disable once UnusedTypeParameter
public interface IAccountant<T> where T : class
{
    Task CalculateAsync(int worldId, CancellationToken ct);
    Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct);
}

[ExcludeFromCodeCoverage]
public class Accountant<T>(IServiceProvider serviceProvider, ILogger<Accountant<T>> logger)
    : BackgroundService, IAccountant<T> where T : class
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var worlds = GetWorlds();
                if (!worlds.Any())
                    throw new DataException("Failed to find any world Ids. This should never happen");

                var accountingTasks = new List<Task>();
                foreach (var world in worlds)
                {
                    logger.LogInformation("Opening ledger to update {Type} updates for world id/name: {Id}/{Name}",
                        typeof(T), world.Id, world.Name);
                    accountingTasks.Add(CalculateAsync(world.GetId(), ct));
                }

                await Task.WhenAll(accountingTasks);
            }
            catch (DataException ex)
            {
                logger.LogCritical($"A critical error occured: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"An unexpected exception occured during the accounting process: {ex.Message}");
            }

            var delay = TimeSpan.FromMinutes(5);
            logger.LogInformation($"Awaiting delay of {delay}");
            await Task.Delay(delay, ct);
        }
    }

    public async Task CalculateAsync(int worldId, CancellationToken ct = default)
    {
        var idList = await GetIdsToUpdate(worldId);
        if (!idList.Any())
        {
            logger.LogInformation($"Nothing to calculate for {worldId}");
            return;
        }

        if (ct.IsCancellationRequested)
        {
            logger.LogInformation($"Cancellation of the task by the user. Putting away the books for {worldId}");
            return;
        }

        try
        {
            await ComputeListAsync(worldId, idList, ct);
            logger.LogInformation($"Boss, books are closed for world {worldId}");
        }
        catch (Exception e)
        {
            logger.LogError("Failed to balance the books for world {WorldId}: {Error}", worldId, e.Message);
        }
    }

    protected List<WorldPoco> GetWorlds()
    {
        using var scope = serviceProvider.CreateScope();
        var worldRepo = scope.ServiceProvider.GetRequiredService<IWorldRepository>();
        return worldRepo.GetAll().ToList();
    }

    public Task ComputeListAsync(int worldId, List<int> idList) =>
        ComputeListAsync(worldId, idList, CancellationToken.None);

    public virtual Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
        => throw new NotImplementedException();

    public virtual int GetDataFreshnessInHours() => throw new NotImplementedException();

    public virtual Task<List<int>> GetIdsToUpdate(int worldId) => Task.FromResult(new List<int>());
}