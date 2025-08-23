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

public interface IAccountant
{
    Task CalculateAsync(int worldId, CancellationToken ct);
    Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct);
}

[ExcludeFromCodeCoverage]
public class Accountant(IServiceProvider serviceProvider, ILogger<Accountant> logger)
    : BackgroundService, IAccountant
{
    protected readonly IServiceProvider ServiceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Starting accountant service. Fetching available worlds first.");
                var worlds = GetWorlds();
                if (!worlds.Any())
                {
                    logger.LogCritical("Failed to find any world Ids. This should never happen.");
                    throw new DataException("No world IDs found.");
                }

                logger.LogInformation("Found {Count} worlds to process.", worlds.Count);

                var tasks = worlds.Select(world =>
                {
                    logger.LogInformation("Processing updates for {Type} in world: {Id}, name: {Name}",
                        nameof(Accountant), world.Id, world.Name);

                    return Task.Run(() => CalculateAsync(world.Id, ct), ct);
                });

                await Task.WhenAll(tasks);
            }
            catch (DataException ex)
            {
                logger.LogCritical("Critical error: {Message}", ex.Message);
                break;
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error during processing: {Message}", ex.Message);
            }

            var delay = TimeSpan.FromMinutes(5);
            logger.LogInformation("Delaying for {Delay}", delay);
            try
            {
                await Task.Delay(delay, ct);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    public async Task CalculateAsync(int worldId, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            logger.LogInformation($"Cancellation of the task by the user. Putting away the books for {worldId}");
            return;
        }

        var idList = await GetIdsToUpdate(worldId);
        if (!idList.Any())
        {
            logger.LogInformation($"Nothing to calculate for {worldId}");
            return;
        }

        try
        {
            var batcher = new Batcher.Batcher<int>();
            var batchesOfIds = batcher.SplitIntoBatchJobs(idList);
            logger.LogInformation("Processing {Count} ids for world {WorldId} in {BatchCount} batches",
                idList.Count,
                worldId,
                batchesOfIds.Count);
            foreach (var idBatch in batchesOfIds)
                await ComputeListAsync(worldId, idBatch, ct);

            logger.LogInformation($"Boss, books are closed for world {worldId}");
        }
        catch (Exception e)
        {
            logger.LogError("Failed to balance the books for world {WorldId}: {Error}", worldId, e.Message);
        }
    }

    protected List<WorldPoco> GetWorlds()
    {
        using var scope = ServiceProvider.CreateScope();
        var worldRepo = scope.ServiceProvider.GetRequiredService<IWorldRepository>();
        return worldRepo.GetAll().ToList();
    }

    public Task ComputeListAsync(int worldId, List<int> idList) =>
        ComputeListAsync(worldId, idList, CancellationToken.None);

    public virtual Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
        => throw new NotImplementedException();

    protected virtual int GetDataFreshnessInHours() => 96;

    public virtual Task<List<int>> GetIdsToUpdate(int worldId) => Task.FromResult(new List<int>());
}