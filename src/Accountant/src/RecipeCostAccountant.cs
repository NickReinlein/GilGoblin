using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
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

    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var gilGoblinDbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var calc = scope.ServiceProvider.GetRequiredService<ICraftingCalculator>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var allRelevantRecipes = gilGoblinDbContext.Recipe.Where(r => idList.Contains(r.Id)).ToList();
            var allCosts = gilGoblinDbContext.RecipeCost.Where(rc => rc.WorldId == worldId
            ).ToList();

            foreach (var recipe in allRelevantRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                var existing = allCosts.FirstOrDefault(c => c.GetId() == recipe.Id);
                if (existing is not null && existing.Updated - DateTimeOffset.Now <= GetDataFreshnessInHours())
                    continue;

                var newCost = await ComputeAsync(worldId, recipe.Id, calc);
                if (newCost is not null)
                    await costRepo.Add(newCost);
            }
        }
        catch (TaskCanceledException)
        {
            Logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
        }
        catch (Exception ex)
        {
            Logger.LogError($"An unexpected exception occured during accounting process: {ex.Message}");
        }
    }

    public override async Task<RecipeCostPoco?> ComputeAsync(int worldId, int recipeId, ICraftingCalculator calc)
    {
        try
        {
            var calculatedCost = await calc.CalculateCraftingCostForRecipe(worldId, recipeId);
            if (calculatedCost <= 1)
                throw new DataException();

            return new RecipeCostPoco
            {
                WorldId = worldId, RecipeId = recipeId, Cost = calculatedCost, Updated = DateTimeOffset.UtcNow
            };
        }
        catch (Exception e)
        {
            var message = $"Failed to calculate crafting cost of recipe {recipeId} world {worldId}: {e.Message}";
            Logger.LogError(message);
            return null;
        }
    }

    public static TimeSpan GetDataFreshnessInHours() => TimeSpan.FromHours(48);

    public override List<int> GetWorldIds() => new() { 34 };

    public override List<int> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            using var scope = ScopeFactory.CreateScope();
            var gilGoblinDbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();

            var priceCosts = gilGoblinDbContext.RecipeCost.Where(cp => cp.WorldId == worldId).ToList();
            var priceCostIds = priceCosts.Select(i => i.GetId()).ToList();
            var prices = gilGoblinDbContext.Price.Where(cp => cp.WorldId == worldId).ToList();
            var priceIds = prices.Select(i => i.GetId()).ToList();
            var missingPriceIds = priceIds.Except(priceCostIds).ToList();
            idsToUpdate.AddRange(missingPriceIds);

            foreach (var price in prices)
            {
                try
                {
                    var current = priceCosts.FirstOrDefault(c => c.GetId() == price.GetId());
                    if (current is null)
                        continue;

                    var timeDelta = price.LastUploadTime - current.Updated.ToUnixTimeMilliseconds();
                    if (timeDelta > timeThreshold)
                        idsToUpdate.Add(current.GetId());
                }
                catch (Exception e)
                {
                    var message =
                        $"Failed during search for price cost: item {price.GetId()}, world {worldId}: {e.Message}";
                    Logger.LogError(message);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
        }

        return idsToUpdate;
    }
}