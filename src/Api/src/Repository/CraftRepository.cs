using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;

namespace GilGoblin.Api.Repository;

public interface ICraftRepository<T> where T : class
{
    Task<List<T>> GetBestAsync(int worldId);
    Task<T?> GetAsync(int worldId, int recipeId);
    List<CraftSummaryPoco> SortByProfitability(IEnumerable<CraftSummaryPoco> crafts);
}

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCostRepository _recipeCostRepository;
    private readonly IRecipeProfitRepository _recipeProfitRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICraftCache _cache;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IRecipeCostRepository recipeCostRepository,
        IRecipeProfitRepository recipeProfitRepository,
        IItemRepository itemRepository,
        ICraftCache cache,
        ILogger<CraftRepository> logger)
    {
        _priceRepository = priceRepo;
        _recipeRepository = recipeRepository;
        _recipeCostRepository = recipeCostRepository;
        _recipeProfitRepository = recipeProfitRepository;
        _itemRepository = itemRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<CraftSummaryPoco>> GetBestAsync(int worldId)
    {
        var crafts = new List<CraftSummaryPoco>();
        var profits =
            _recipeProfitRepository
                .GetAll(worldId)
                .OrderBy(rp => rp.RecipeProfitVsSold)
                .Take(100)
                .ToList();
        var recipeIds = profits.Select(i => i.RecipeId).ToList();

        var allRecipes = _recipeRepository.GetMultiple(recipeIds).ToList();
        foreach (var profit in profits)
        {
            try
            {
                var recipeId = profit.RecipeId;
                var summary = await CreateSummaryAsync(worldId, recipeId, allRecipes);

                crafts.Add(summary);
                _cache.Add((summary.WorldId, summary.Recipe.Id), summary);
            }
            catch (Exception e)
            {
                var message = $"Error creating craft summary: recipe {profit.RecipeId}, world {worldId}: {e.Message}";
                _logger.LogError(message);
            }
        }

        return SortByProfitability(crafts);
    }

    public Task<CraftSummaryPoco?> GetAsync(int worldId, int recipeId)
    {
        try
        {
            var recipe = _recipeRepository.Get(recipeId);
            return recipe is null ? null : CreateSummaryAsync(worldId, recipeId, new List<RecipePoco> { recipe });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get craft {Recipe} for world {World}", recipeId, worldId);
            return null;
        }
    }

    private async Task<CraftSummaryPoco> CreateSummaryAsync(int worldId, int recipeId,
        IEnumerable<RecipePoco> allRecipes)
    {
        var recipeCost = await _recipeCostRepository.GetAsync(worldId, recipeId);
        if (recipeCost is null || recipeCost.Cost <= 0)
            throw new DataException(
                $"Failed to find cost of recipe cost of recipe {recipeId} for world {worldId}");

        var recipe = allRecipes.FirstOrDefault(i => i.Id == recipeId);
        var ingredients = recipe.GetActiveIngredients();
        if (recipe is null || !ingredients.Any())
            throw new DataException($"Failed to find cost of recipe {recipeId} for world {worldId}");

        var item = _itemRepository.Get(recipe.TargetItemId);
        if (item is null)
            throw new DataException($"Failed to find target item of recipe {recipeId} for world {worldId}");

        var price = _priceRepository.Get(worldId, recipe.TargetItemId);
        if (price is null)
            throw new DataException($"Failed to find price of target item for {recipeId} for world {worldId}");

        var summary = new CraftSummaryPoco(
            price,
            item,
            recipeCost.Cost,
            recipe,
            ingredients
        );
        return summary;
    }

    public List<CraftSummaryPoco> SortByProfitability(IEnumerable<CraftSummaryPoco> crafts)
    {
        var craftsList = crafts.ToList();
        if (!craftsList.Any())
            return craftsList;

        try
        {
            var viableCrafts
                = craftsList
                    .Where(i =>
                        i.AverageSold > 1 &&
                        i.AverageListingPrice > 1 &&
                        i.RecipeCost > 1 &&
                        i.Recipe.TargetItemId == i.ItemId)
                    .ToList();
            viableCrafts.Sort();
            return viableCrafts;
        }
        catch (Exception e)
        {
            var message = $"Failed to sort crafts by profitability! Returning unsorted crafts list: {e.Message}";
            _logger.LogError(message);
            return craftsList;
        }
    }
}