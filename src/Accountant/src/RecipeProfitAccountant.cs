using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Accountant;

public interface IRecipeProfitAccountant
{
    public RecipeProfitPoco? CalculateCraftingProfitForQualityRecipe(
        int worldId,
        int recipeId,
        bool isHq,
        IEnumerable<RecipeCostPoco> costs,
        IEnumerable<PricePoco> prices);
}

public class RecipeProfitAccountant(
    IServiceProvider serviceProvider,
    IDataSaver<RecipeProfitPoco> saver,
    ILogger<RecipeProfitAccountant> logger)
    : Accountant<RecipeProfitPoco>(serviceProvider, logger), IRecipeProfitAccountant
{
    public override int GetDataFreshnessInHours() => 96;

    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        if (worldId <= 0 || !idList.Any() || ct.IsCancellationRequested)
            return;

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var allRequestedRecipes = recipeRepo.GetMultiple(idList).ToList();
            var existingProfits = (await profitRepo.GetAllAsync(worldId)).ToList();
            var targetItemIds = allRequestedRecipes.Select(req => req.TargetItemId).ToList();
            var prices = priceRepo.GetMultiple(worldId, targetItemIds, false).ToList();
            var costs = (await costRepo.GetAllAsync(worldId)).ToList();

            var newProfits = new List<RecipeProfitPoco>();
            foreach (var recipe in allRequestedRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                try
                {
                    var recipeId = recipe.Id;
                    logger.LogDebug("Calculating new profits for {RecipeId} for world {WorldId}", recipeId, worldId);
                    foreach (var quality in new[] { true, false })
                    {
                        var profit = existingProfits
                            .FirstOrDefault(c =>
                                c.GetId() == recipeId &&
                                c.WorldId == worldId &&
                                c.IsHq == quality);
                        if (profit is not null)
                        {
                            var age = (DateTimeOffset.UtcNow - profit.LastUpdated).TotalHours;
                            if (age <= GetDataFreshnessInHours())
                                continue;
                        }

                        var calculatedProfit =
                            CalculateCraftingProfitForQualityRecipe(worldId, recipeId, quality, costs, prices);
                        if (calculatedProfit is not null)
                            newProfits.Add(calculatedProfit);
                    }
                }
                catch (Exception)
                {
                    logger.LogWarning("Failed to calculate crafting cost of recipe {RecipeId} for world {WorldId}",
                        recipe.Id,
                        worldId);
                }

                await saver.SaveAsync(newProfits, ct);
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected exception occured during the accounting process for world {WorldId}: {Message}";
            logger.LogError(message, worldId, ex.Message);
        }
    }

    public RecipeProfitPoco? CalculateCraftingProfitForQualityRecipe(
        int worldId,
        int recipeId,
        bool isHq,
        IEnumerable<RecipeCostPoco> costs,
        IEnumerable<PricePoco> prices)
    {
        var cost = costs.FirstOrDefault(c =>
            c.GetId() == recipeId &&
            c.WorldId == worldId &&
            c.IsHq == isHq);
        if (cost is null)
        {
            logger.LogError("Failed to match crafting cost of recipe {RecipeId} for world {WorldId}",
                recipeId,
                worldId);
            return null;
        }

        var price = prices.FirstOrDefault(c =>
            c.GetId() == recipeId &&
            c.WorldId == worldId &&
            c.IsHq == isHq);
        if (price is null)
        {
            logger.LogError("Failed to match market price of recipe {RecipeId} for world {WorldId}", recipeId,
                worldId);
            return null;
        }

        var profitAmount = (int)price.GetBestPriceCost() - cost.Amount;

        return new RecipeProfitPoco(worldId, recipeId, isHq, profitAmount, DateTimeOffset.UtcNow.DateTime);
    }

    public override async Task<List<int>> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            await using var scope = serviceProvider.CreateAsyncScope();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();

            var costs = (await costRepo.GetAllAsync(worldId)).ToList();
            var costIds = costs.Select(i => i.GetId()).ToList();
            var recipeProfitPocos = await profitRepo.GetMultipleAsync(worldId, costIds);
            var existingProfits = recipeProfitPocos.ToList();

            foreach (var cost in costs)
            {
                var id = cost.GetId();
                var existing = existingProfits.FirstOrDefault(e =>
                    e.GetId() == id &&
                    e.WorldId == worldId &&
                    cost.IsHq);
                if (existing is null)
                {
                    idsToUpdate.Add(id);
                    continue;
                }

                var age = (DateTimeOffset.UtcNow - existing.LastUpdated).TotalHours;
                if (age >= GetDataFreshnessInHours())
                    idsToUpdate.Add(id);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Failed to get the Ids to update for world {WorldId}: {Message}", worldId, e.Message);
        }

        return idsToUpdate.Distinct().ToList();
    }
}