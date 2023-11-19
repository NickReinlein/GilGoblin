using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Accountant;

public class RecipeProfitAccountant : Accountant<RecipeProfitPoco>, IRecipeProfitAccountant
{
    public RecipeProfitAccountant(
        IServiceScopeFactory scopeFactory,
        ILogger<Accountant<RecipeProfitPoco>> logger) :
        base(scopeFactory, logger)
    {
    }

    public override async Task ComputeListAsync(int worldId, List<int> idList, CancellationToken ct)
    {
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();
            var allRequestedRecipes = recipeRepo.GetMultiple(idList).ToList();
            var existingProfits = profitRepo.GetAll(worldId).ToList();
            var prices = priceRepo.GetMultiple(worldId, idList).ToList();
            var costs = costRepo.GetAll(worldId).ToList();

            foreach (var recipe in allRequestedRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                var recipeId = recipe.Id;
                var profit = existingProfits.FirstOrDefault(c => c.GetId() == recipeId);
                if (profit is not null && profit.Updated - DateTimeOffset.Now <= GetDataFreshnessInHours())
                    continue;

                var newProfit = ComputeAsync(worldId, recipeId, recipe, costs, prices);
                if (newProfit is not null)
                    await profitRepo.Add(newProfit);
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

    public RecipeProfitPoco? ComputeAsync(
        int worldId,
        int recipeId,
        RecipePoco recipe,
        IEnumerable<RecipeCostPoco> costs,
        IEnumerable<PricePoco> prices)
    {
        var cost = costs.FirstOrDefault(c => c.GetId() == recipeId);
        if (cost is null)
        {
            var message = $"Failed to match crafting cost of recipe {recipeId} for world {worldId}";
            Logger.LogError(message);
            return null;
        }

        var price = prices.FirstOrDefault(c => c.GetId() == recipe.TargetItemId);
        if (price is null)
        {
            var message = $"Failed to match market price of recipe {recipeId} for world {worldId}";
            Logger.LogError(message);
            return null;
        }

        var recipeProfitVsListings = (int)price.AverageListingPrice - cost.Cost;
        var recipeProfitVsSold = (int)price.AverageSold - cost.Cost;

        var newProfit = new RecipeProfitPoco
        {
            WorldId = worldId,
            RecipeId = recipeId,
            RecipeProfitVsListings = recipeProfitVsListings,
            RecipeProfitVsSold = recipeProfitVsSold,
            Updated = DateTimeOffset.UtcNow
        };
        return newProfit;
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
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();

            var currentRecipeProfits = profitRepo.GetAll(worldId).ToList();
            var currentRecipeIds = currentRecipeProfits.Select(i => i.GetId()).ToList();
            var prices = priceRepo.GetAll(worldId).ToList();
            var priceIds = prices.Select(i => i.GetId()).ToList();
            var missingPriceIds = priceIds.Except(currentRecipeIds).ToList();
            idsToUpdate.AddRange(missingPriceIds);

            var costs = costRepo.GetAll(worldId).ToList();
            var costIds = costs.Select(i => i.GetId()).ToList();
            var existingProfits = profitRepo.GetMultiple(worldId, costIds).ToList();

            foreach (var cost in costs)
            {
                try
                {
                    var existing = existingProfits.FirstOrDefault(e => e.GetId() == cost.GetId());
                    if (existing is not null && existing.Updated - DateTimeOffset.Now <= GetDataFreshnessInHours())
                        continue;

                    idsToUpdate.Add(cost.GetId());
                }
                catch (Exception e)
                {
                    var message =
                        $"Failed during search for price profit: item {cost.GetId()}, world {worldId}: {e.Message}";
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

public interface IRecipeProfitAccountant
{
    RecipeProfitPoco? ComputeAsync(
        int worldId,
        int recipeId,
        RecipePoco recipe,
        IEnumerable<RecipeCostPoco> costs,
        IEnumerable<PricePoco> prices);
}