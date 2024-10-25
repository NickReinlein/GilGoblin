using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Accountant;

public class RecipeCostAccountant(
    IServiceProvider serviceProvider,
    IDataSaver<RecipeCostPoco> saver,
    ILogger<RecipeCostAccountant> logger)
    : Accountant<RecipeCostPoco>(serviceProvider, logger)
{
    public override int GetDataFreshnessInHours() => 96;

    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        try
        {
            var (recipeCosts, relevantRecipes) = await GetRecipesAndCosts(worldId, idList);

            logger.LogDebug(
                "Found {Count} relevant recipes for world {WorldId}, compared to the {ParamCount} requested recipes",
                relevantRecipes.Count,
                worldId,
                idList.Count);
            logger.LogDebug("Found {Count} costs for world {worldId}", recipeCosts.Count, worldId);

            await using var scope = serviceProvider.CreateAsyncScope();
            var calc = scope.ServiceProvider.GetRequiredService<ICraftingCalculator>();

            var newCosts = new List<RecipeCostPoco>();
            foreach (var recipe in relevantRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                try
                {
                    var recipeId = recipe.Id;
                    logger.LogDebug("Calculating new costs for {RecipeId} for world {WorldId}", recipeId, worldId);
                    foreach (var quality in new[] { true, false })
                    {
                        var current = recipeCosts.FirstOrDefault(c =>
                            c.RecipeId == recipeId && c.WorldId == worldId && c.IsHq == quality);
                        if (current is not null)
                        {
                            var age = (DateTimeOffset.UtcNow - current.LastUpdated).TotalHours;
                            if (age <= GetDataFreshnessInHours())
                                continue;
                        }

                        var result = await calc.CalculateCraftingCostForRecipe(worldId, recipeId, quality);
                        if (result > CraftingCalculator.ErrorDefaultCost - 20000 || result < 0)
                        {
                            throw new Exception(
                                $"Failed to calculate crafting cost of recipe {recipeId} for world {worldId}, quality {quality}");
                        }

                        var newRecipeCost = new RecipeCostPoco(
                            recipeId,
                            worldId,
                            quality,
                            result,
                            DateTimeOffset.UtcNow.DateTime);
                        newCosts.Add(newRecipeCost);
                    }
                }
                catch (Exception)
                {
                    logger.LogWarning("Failed to calculate crafting cost of recipe {RecipeId} for world {WorldId}",
                        recipe.Id,
                        worldId);
                }
            }

            await saver.SaveAsync(newCosts, ct);
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
        }
        catch (Exception ex)
        {
            logger.LogError(
                "An unexpected exception occured during the accounting process for world {WorldId}: {Message}",
                worldId,
                ex.Message);
        }
    }

    private async Task<(List<RecipeCostPoco> existingRecipeCosts, List<RecipePoco> allRelevantRecipes)>
        GetRecipesAndCosts(int worldId, List<int> idList)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
        var existingRecipeCosts = await costRepo.GetAllAsync(worldId);

        var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
        var allRelevantRecipes = recipeRepo.GetMultiple(idList).ToList();
        return (existingRecipeCosts, allRelevantRecipes);
    }

    public override async Task<List<int>> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            await using var scope = serviceProvider.CreateAsyncScope();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var recipes = recipeRepo.GetAll().ToList();
            var currentRecipeCosts = await costRepo.GetAllAsync(worldId);
            var prices = priceRepo.GetAll(worldId).ToList();
            var priceIds = prices.Select(i => i.GetId()).ToList();
            var currentRecipeIds = currentRecipeCosts
                .Where(r => r.WorldId == worldId)
                .Select(c => c.RecipeId)
                .ToList();
            var missingPriceIds = priceIds.Except(currentRecipeIds).ToList();
            idsToUpdate.AddRange(missingPriceIds);

            foreach (var recipe in recipes)
            {
                var current = currentRecipeCosts.FirstOrDefault(c =>
                    c.RecipeId == recipe.Id &&
                    c.WorldId == worldId);
                if (current is not null)
                {
                    logger.LogDebug("Found recipe cost for Recipe {RecipeId} for world {WorldId}", recipe.Id,
                        worldId);
                    var age = (DateTimeOffset.UtcNow - current.LastUpdated).TotalHours;
                    if (age <= GetDataFreshnessInHours())
                        continue; // Fresh data is skipped
                }

                idsToUpdate.Add(recipe.Id);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
        }

        return idsToUpdate.Distinct().ToList();
    }
}