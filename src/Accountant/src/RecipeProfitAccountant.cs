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
    int? CalculateProfit(int recipeId,
        int worldId,
        bool isHq,
        int targetItemId,
        IEnumerable<RecipeCostPoco> costs,
        IEnumerable<PricePoco> prices);
}

public class RecipeProfitAccountant(
    IServiceProvider serviceProvider,
    IDataSaver<RecipeProfitPoco> saver,
    ILogger<RecipeProfitAccountant> logger)
    : Accountant(serviceProvider, logger), IRecipeProfitAccountant
{
    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        if (worldId <= 0 || !idList.Any() || ct.IsCancellationRequested)
            return;

        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();

            var costsList = await costRepo.GetMultipleAsync(worldId, idList);
            if (!costsList.Any())
                return;

            var currentProfits = await profitRepo.GetMultipleAsync(worldId, idList);
            var recipes = recipeRepo
                .GetMultiple(idList)
                .ToList();
            var prices = priceRepo.GetAll(worldId).ToList();

            var newProfits = new List<RecipeProfitPoco>();
            foreach (var cost in costsList)
            {
                try
                {
                    var currentProfit = currentProfits.FirstOrDefault(c =>
                        c.RecipeId == cost.RecipeId &&
                        c.WorldId == cost.WorldId &&
                        c.IsHq == cost.IsHq);
                    if (currentProfit is not null)
                    {
                        var age = (DateTimeOffset.UtcNow - currentProfit.LastUpdated).TotalHours;
                        if (age <= GetDataFreshnessInHours())
                            continue;
                    }

                    var recipe = recipes.FirstOrDefault(r => r.Id == cost.RecipeId);
                    if (recipe is null)
                    {
                        logger.LogWarning("Failed to find recipe {RecipeId}", cost.RecipeId);
                        continue;
                    }

                    var calculatedProfit = CalculateProfit(
                        cost.RecipeId,
                        cost.WorldId,
                        cost.IsHq,
                        recipe.TargetItemId,
                        costsList,
                        prices);
                    if (!calculatedProfit.HasValue)
                        continue;

                    var profitToUpdate = CalculateProfitToUpdate(currentProfit, calculatedProfit.Value, cost);
                    newProfits.Add(profitToUpdate);
                }
                catch (Exception)
                {
                    logger.LogWarning("Failed to calculate crafting cost of recipe {RecipeId} for world {WorldId}",
                        cost.RecipeId,
                        cost.WorldId);
                }
            }

            await saver.SaveAsync(newProfits, ct);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected exception occured during the accounting process for world {WorldId}: {Message}";
            logger.LogError(message, worldId, ex.Message);
        }
    }

    private static RecipeProfitPoco CalculateProfitToUpdate(
        RecipeProfitPoco? profitPoco,
        int calculatedProfit,
        RecipeCostPoco cost)
    {
        return profitPoco is not null
            ? profitPoco with { Amount = calculatedProfit, LastUpdated = DateTimeOffset.UtcNow }
            : new RecipeProfitPoco(cost.RecipeId, cost.WorldId, cost.IsHq, calculatedProfit, DateTimeOffset.UtcNow);
    }

    public int? CalculateProfit(int recipeId, int worldId, bool isHq, int targetItemId,
        IEnumerable<RecipeCostPoco> costs, IEnumerable<PricePoco> prices)
    {
        var cost = costs.FirstOrDefault(c =>
            c.RecipeId == recipeId &&
            c.WorldId == worldId &&
            c.IsHq == isHq);
        if (cost is null)
        {
            logger.LogError("Failed to match crafting cost of recipe {RecipeId} for world {WorldId}, isHq:{IsHq}",
                recipeId,
                worldId,
                isHq);
            return null;
        }

        var price = prices.FirstOrDefault(c =>
            c.ItemId == targetItemId &&
            c.WorldId == worldId &&
            c.IsHq == isHq);
        if (price is null)
        {
            logger.LogDebug("Failed to match market price of recipe {RecipeId} for world {WorldId}", recipeId,
                worldId);
            return null;
        }

        var salePrice = (int)price.GetBestPriceAmount();

        return salePrice - cost.Amount;
    }

    public override async Task<List<int>> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            await using var scope = ServiceProvider.CreateAsyncScope();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();

            var costs = (await costRepo.GetAllAsync(worldId)).ToList();
            var costIds = costs.Select(i => i.GetId()).ToList();
            var recipeProfitPocos = await profitRepo.GetMultipleAsync(worldId, costIds);
            var existingProfits = recipeProfitPocos.ToList();

            foreach (var cost in costs)
            {
                var existing = existingProfits.FirstOrDefault(e =>
                    e.Id == cost.Id &&
                    e.WorldId == cost.WorldId &&
                    e.IsHq == cost.IsHq);
                if (existing is null)
                {
                    idsToUpdate.Add(cost.Id);
                    continue;
                }

                var age = (DateTimeOffset.UtcNow - existing.LastUpdated).TotalHours;
                if (age >= GetDataFreshnessInHours())
                    idsToUpdate.Add(cost.Id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get the Ids to update for world {WorldId}", worldId);
        }

        return idsToUpdate.Distinct().ToList();
    }
}