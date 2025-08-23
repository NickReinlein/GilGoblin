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
    : Accountant(serviceProvider, logger)
{
    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        if (worldId <= 0 || !idList.Any() || ct.IsCancellationRequested)
            return;

        try
        {
            var (recipeCosts, relevantRecipes) = await GetRecipesAndCosts(worldId, idList);

            logger.LogInformation(
                "Found {Count} relevant recipes for world {WorldId}, compared to the {ParamCount} requested recipes",
                relevantRecipes.Count,
                worldId,
                idList.Count);
            logger.LogInformation("Found {Count} costs for world {worldId}", recipeCosts.Count, worldId);

            await using var scope = ServiceProvider.CreateAsyncScope();
            var calc = scope.ServiceProvider.GetRequiredService<ICraftingCalculator>();

            var allCostsToUpdate = new List<RecipeCostPoco>();
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
                        var currentCost = recipeCosts.FirstOrDefault(c =>
                            c.RecipeId == recipeId &&
                            c.WorldId == worldId &&
                            c.IsHq == quality);
                        if (currentCost is not null)
                        {
                            var age = (DateTimeOffset.UtcNow - currentCost.LastUpdated).TotalHours;
                            if (age <= GetDataFreshnessInHours())
                                continue;
                        }

                        var calculateCost = await calc.CalculateCraftingCostForRecipe(worldId, recipeId, quality);
                        if (calculateCost > CraftingCalculator.ErrorDefaultCost - 20000 || calculateCost < 1)
                        {
                            throw new Exception(
                                $"Failed to calculate crafting cost of recipe {recipeId} for world {worldId}, quality {quality}");
                        }

                        var costToUpdate =
                            CalculateCostToUpdate(currentCost, worldId, recipeId, quality, calculateCost);
                        allCostsToUpdate.Add(costToUpdate);
                    }
                }
                catch (Exception)
                {
                    logger.LogWarning("Failed to calculate crafting cost of recipe {RecipeId} for world {WorldId}",
                        recipe.Id,
                        worldId);
                }
            }

            await saver.SaveAsync(allCostsToUpdate, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "An unexpected exception occured during the accounting process for world {WorldId}: {Message}",
                worldId,
                ex.Message);
        }
    }

    private static RecipeCostPoco CalculateCostToUpdate(
        RecipeCostPoco? currentCost,
        int worldId,
        int recipeId,
        bool quality,
        int result)
    {
        return currentCost is not null
            ? currentCost with { Amount = result, LastUpdated = DateTimeOffset.UtcNow }
            : new RecipeCostPoco(recipeId, worldId, quality, result, DateTimeOffset.UtcNow);
    }

    public override async Task<List<int>> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            await using var scope = ServiceProvider.CreateAsyncScope();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var recipes = recipeRepo.GetAll().ToList();
            var currentRecipeCosts = await costRepo.GetAllAsync(worldId);

            foreach (var recipe in recipes)
            {
                var existing = currentRecipeCosts.Where(c =>
                        c.RecipeId == recipe.Id &&
                        c.WorldId == worldId &&
                        c.IsHq == recipe.CanHq)
                    .ToList();

                if (!existing.Any())
                {
                    idsToUpdate.Add(recipe.Id);
                    continue;
                }

                foreach (var cost in existing)
                {
                    logger.LogDebug("Found recipe cost for Recipe {RecipeId} for world {WorldId}", recipe.Id,
                        worldId);
                    var age = (DateTimeOffset.UtcNow - cost.LastUpdated).TotalHours;
                    if (age <= GetDataFreshnessInHours())
                        continue; // Fresh data is skipped
                    idsToUpdate.Add(recipe.Id);
                    break;
                }
            }

            return idsToUpdate.Distinct().ToList();
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
        }

        return idsToUpdate.Distinct().ToList();
    }

    private async Task<(List<RecipeCostPoco> existingRecipeCosts, List<RecipePoco> allRelevantRecipes)>
        GetRecipesAndCosts(int worldId, List<int> idList)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
        var existingRecipeCosts = await costRepo.GetMultipleAsync(worldId, idList);

        var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
        var allRelevantRecipes = recipeRepo.GetMultiple(idList).ToList();
        return (existingRecipeCosts, allRelevantRecipes);
    }
}