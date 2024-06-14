using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Accountant;

public class RecipeCostAccountant : Accountant<RecipeCostPoco>
{
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
            var calc = scope.ServiceProvider.GetRequiredService<ICraftingCalculator>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var existingRecipeCosts = costRepo.GetAll(worldId).ToList();
            var allRelevantRecipes = recipeRepo.GetMultiple(idList).ToList();

            foreach (var recipe in allRelevantRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                try
                {
                    var recipeId = recipe.Id;
                    var existing = existingRecipeCosts.FirstOrDefault(c => c.GetId() == recipeId);
                    if (existing is not null && existing.Updated - DateTimeOffset.Now <= GetDataFreshnessInHours())
                        continue;

                    var calculatedCost = await calc.CalculateCraftingCostForRecipe(worldId, recipeId);
                    if (calculatedCost <= 1)
                    {
                        var message = $"Failed to calculate crafting cost of recipe {recipeId} world {worldId}";
                        Logger.LogError(message);
                        continue;
                    }

                    var newCost = new RecipeCostPoco
                    {
                        WorldId = worldId,
                        RecipeId = recipeId,
                        Cost = calculatedCost,
                        Updated = DateTimeOffset.UtcNow
                    };
                    await costRepo.Add(newCost);
                }
                catch (Exception e)
                {
                    var message =
                        $"Failed to calculate crafting cost of recipe {recipe.Id} for world {worldId}: {e.Message}";
                    Logger.LogError(message);
                }
            }
        }
        catch (TaskCanceledException)
        {
            Logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
        }
        catch (Exception ex)
        {
            Logger.LogError(
                $"An unexpected exception occured during the accounting process for world {worldId}: {ex.Message}");
        }
    }

    public static TimeSpan GetDataFreshnessInHours() => TimeSpan.FromHours(48);

    public override List<int> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            using var scope = ScopeFactory.CreateScope();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var recipes = recipeRepo.GetAll().ToList();
            var currentRecipeCosts = costRepo.GetAll(worldId).ToList();
            var prices = priceRepo.GetAll(worldId).ToList();
            var currentRecipeIds = currentRecipeCosts.Select(i => i.GetId()).ToList();
            var priceIds = prices.Select(i => i.GetId()).ToList();
            var missingPriceIds = priceIds.Except(currentRecipeIds).ToList();
            idsToUpdate.AddRange(missingPriceIds);

            foreach (var recipe in recipes)
            {
                var current = currentRecipeCosts.FirstOrDefault(c => c.GetId() == recipe.Id);
                if (current is not null &&
                    current.Updated - DateTimeOffset.Now <= GetDataFreshnessInHours())
                    continue;

                idsToUpdate.Add(recipe.Id);
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
        }

        return idsToUpdate.Distinct().ToList();
    }
}