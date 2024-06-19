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
    private const string ageMessage = "Recipe profit calculation is only {Age} hours old and fresh, " +
                                      "therefore not updating for recipe {RecipeId} for world {WorldId}";

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
            var targetItemIds = allRequestedRecipes.Select(req => req.TargetItemId).ToList();
            var prices = priceRepo.GetMultiple(worldId, targetItemIds).ToList();
            var costs = costRepo.GetAll(worldId).ToList();

            foreach (var recipe in allRequestedRecipes)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                var recipeId = recipe.Id;
                var profit = existingProfits.FirstOrDefault(c => c.GetId() == recipeId);
                if (profit is not null)
                {
                    Logger.LogDebug("Found recipe profit for Recipe {RecipeId} for world {WorldId}", recipe.Id,
                        worldId);
                    var age = (DateTimeOffset.Now - profit.Updated).TotalHours;
                    if (age <= GetDataFreshnessInHours())
                    {
                        Logger.LogDebug(ageMessage, age, recipe.Id, worldId);
                        continue;
                    }
                }

                var newProfit = ComputeAsync(worldId, recipeId, recipe, costs, prices);
                if (newProfit is not null)
                    await profitRepo.AddAsync(newProfit);
            }
        }
        catch (TaskCanceledException)
        {
            Logger.LogInformation("Task was cancelled by user. Putting away the books, boss!");
        }
        catch (Exception ex)
        {
            var message =
                $"An unexpected exception occured during the accounting process for world {worldId}: {ex.Message}";
            Logger.LogError(message);
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
            Logger.LogError("Failed to match crafting cost of recipe {RecipeId} for world {WorldId}",
                recipeId,
                worldId);
            return null;
        }

        var price = prices.FirstOrDefault(c => c.GetId() == recipe.TargetItemId);
        if (price is null)
        {
            Logger.LogError("Failed to match market price of recipe {RecipeId} for world {WorldId}", recipeId, worldId);
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

    public override int GetDataFreshnessInHours() => 48;

    public override List<int> GetIdsToUpdate(int worldId)
    {
        var idsToUpdate = new List<int>();
        try
        {
            if (worldId < 1)
                throw new Exception("World Id is invalid");

            using var scope = ScopeFactory.CreateScope();
            var profitRepo = scope.ServiceProvider.GetRequiredService<IRecipeProfitRepository>();
            var costRepo = scope.ServiceProvider.GetRequiredService<IRecipeCostRepository>();

            var costs = costRepo.GetAll(worldId).ToList();
            var costIds = costs.Select(i => i.GetId()).ToList();
            var existingProfits = profitRepo.GetMultiple(worldId, costIds).ToList();

            foreach (var cost in costs)
            {
                var id = cost.GetId();
                var existing = existingProfits.FirstOrDefault(e => e.GetId() == id);
                if (existing is null)
                {
                    idsToUpdate.Add(id);
                    continue;
                }

                var age = (DateTimeOffset.Now - existing.Updated).TotalHours;
                if (age >= GetDataFreshnessInHours())
                    idsToUpdate.Add(id);
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to get the Ids to update for world {WorldId}: {Message}", worldId, e.Message);
        }

        return idsToUpdate.Distinct().ToList();
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