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

public class RecipeCostAccountant(IServiceScopeFactory scopeFactory, ILogger<Accountant<RecipeCostPoco>> logger)
    : Accountant<RecipeCostPoco>(scopeFactory, logger)
{
    public override int GetDataFreshnessInHours() => 48;

    private const string ageMessage = "Recipe cost calculation is only {Age} hours old and fresh, " +
                                      "therefore not updating for recipe {RecipeId} for world {WorldId}";

    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var calc = scope.ServiceProvider.GetRequiredService<ICraftingCalculator>();

            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var existingRecipeCosts = costRepo.GetAll(worldId).ToList();

            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var allRelevantRecipes = recipeRepo.GetMultiple(idList).ToList();

            Logger.LogInformation(
                "Found {Count} relevant recipes for world {WorldId}, compared to the {ParamCount} requested recipes",
                allRelevantRecipes.Count,
                worldId,
                idList.Count);
            Logger.LogInformation("Found {Count} costs for world {worldId}", existingRecipeCosts.Count, worldId);

            foreach (var recipe in allRelevantRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                try
                {
                    var recipeId = recipe.Id;
                    var current = existingRecipeCosts.FirstOrDefault(c => c.GetId() == recipeId);
                    if (current is not null)
                    {
                        Logger.LogDebug("Found current recipe cost for Recipe {RecipeId} for world {WorldId}",
                            recipe.Id, worldId);
                        var age = (DateTimeOffset.Now - current.Updated).TotalHours;
                        if (age <= GetDataFreshnessInHours())
                        {
                            Logger.LogDebug(ageMessage, age, recipe.Id, worldId);
                            continue;
                        }
                    }

                    Logger.LogDebug("Calculating new cost for {RecipeId} for world {WorldId}", recipeId, worldId);
                    var calculatedCost = await calc.CalculateCraftingCostForRecipe(worldId, recipeId);
                    if (calculatedCost is <= 1 or >= 1147483647)
                    {
                        Logger.LogError(
                            "Failed to calculate crafting cost of recipe {RecipeId} world {WorldId}: {Cost}",
                            recipeId, worldId, calculatedCost.ToString());
                        continue;
                    }

                    var newCost = new RecipeCostPoco
                    {
                        WorldId = worldId,
                        RecipeId = recipeId,
                        Cost = calculatedCost,
                        Updated = DateTimeOffset.UtcNow
                    };
                    Logger.LogDebug("Cost of recipe {RecipeId} for world {WorldId} is successfully calculated: {Cost}",
                        newCost.RecipeId, newCost.WorldId, newCost.Cost);
                    await costRepo.AddAsync(newCost);
                }
                catch (Exception e)
                {
                    const string message =
                        "Failed to calculate crafting cost of recipe {RecipeId} for world {WorldId}: {Message}";
                    Logger.LogWarning(message, recipe.Id, worldId, e.Message);
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
                "An unexpected exception occured during the accounting process for world {WorldId}: {Message}",
                worldId, ex.Message);
        }
    }

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
                if (current is not null)
                {
                    Logger.LogDebug("Found recipe cost for Recipe {RecipeId} for world {WorldId}", recipe.Id, worldId);
                    var age = (DateTimeOffset.Now - current.Updated).TotalHours;
                    if (age <= GetDataFreshnessInHours())
                    {
                        Logger.LogDebug(ageMessage, age, recipe.Id, worldId);
                        continue;
                    }
                }

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