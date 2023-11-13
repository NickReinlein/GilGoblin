using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Accountant;

public class RecipeCostAccountant : Accountant<RecipeCostPoco>
{
    public const long timeThreshold = 1000;

    public RecipeCostAccountant(
        IServiceScopeFactory scopeFactory,
        ILogger<Accountant<RecipeCostPoco>> logger) :
        base(scopeFactory, logger)
    {
    }

    protected override async Task ComputeAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var gilGoblinDbContext = GetDbContext();
                var pricesToCompute = gilGoblinDbContext.Price.Where(i => idList.Contains(i.GetId())).ToList();
                foreach (var price in pricesToCompute)
                {
                    
                }

                var delay = TimeSpan.FromMinutes(5);
                Logger.LogInformation($"Awaiting delay of {delay}");
                await Task.Delay(delay, ct);
            }
            catch (TaskCanceledException)
            {
                Logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError($"An unexpected exception occured during accounting process: {ex.Message}");
            }
        }
    }

    protected override List<int> GetWorldIds() => new() { 34 };

    protected override List<int> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            using var gilGoblinDbContext = GetDbContext();

            var priceCosts = gilGoblinDbContext.RecipeCost.Where(cp => cp.WorldId == worldId).ToList();
            var priceCostIds = priceCosts.Select(i => i.GetId()).ToList();
            var prices = gilGoblinDbContext.Price.Where(cp => cp.WorldId == worldId).ToList();
            var priceIds = prices.Select(i => i.GetId()).ToList();
            var missingPriceIds = priceIds.Except(priceCostIds).ToList();
            idsToUpdate.AddRange(missingPriceIds);

            foreach (var price in prices)
            {
                var current = priceCosts.Find(c => c.GetId() == price.GetId());
                var timeDelta = price.LastUploadTime - current.Updated.ToUnixTimeMilliseconds();
                if (timeDelta > timeThreshold)
                    idsToUpdate.Add(current.GetId());
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
        }

        return idsToUpdate;
    }
}