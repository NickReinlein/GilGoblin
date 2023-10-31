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

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly ICraftingCalculator _calc;
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCostRepository _recipeCostRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICraftCache _cache;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IRecipeCostRepository recipeCostRepository,
        IItemRepository itemRepository,
        ICraftCache cache,
        ILogger<CraftRepository> logger)
    {
        _calc = calc;
        _priceRepository = priceRepo;
        _recipeRepository = recipeRepository;
        _recipeCostRepository = recipeCostRepository;
        _itemRepository = itemRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<CraftSummaryPoco> GetBestCraftForItem(int worldId, int itemId)
    {
        var cached = _cache.Get((worldId, itemId));
        if (cached is not null)
            return cached;

        var (recipeId, craftingCost) = await _calc.CalculateCraftingCostForItem(worldId, itemId);
        if (recipeId < 1 || craftingCost.IsErrorCost())
            return null;

        var recipe = _recipeRepository.Get(recipeId);
        if (recipe is null)
            return null;
        var ingredients = recipe.GetActiveIngredients();

        var price = _priceRepository.Get(worldId, itemId);
        var item = _itemRepository.Get(itemId);

        var craftSummaryPoco = new CraftSummaryPoco(
            price,
            item,
            craftingCost,
            recipe,
            ingredients
        );
        _cache.Add((worldId, itemId), craftSummaryPoco);
        return craftSummaryPoco;
    }

    public async Task<List<CraftSummaryPoco>> GetBestCraftsForWorld(int worldId)
    {
        var crafts = new List<CraftSummaryPoco>();
        var allRecipes = _recipeRepository.GetAll().ToList();
        foreach (var recipe in allRecipes)
        {
            try
            {
                var summary = await GetSummaryForRecipe(worldId, recipe);
                crafts.Add(summary);
            }
            catch (Exception e)
            {
                var message =
                    $"An error occured getting the craft summary, recipe {recipe.Id}: {e.Message}";
                _logger.LogError(message);
            }
        }

        return SortByProfitability(crafts);
    }

    private static List<CraftSummaryPoco> SortByProfitability(IEnumerable<CraftSummaryPoco> crafts)
    {
        var viableCrafts =
            crafts.Where(i =>
                i.AverageSold > 1 &&
                i.AverageListingPrice > 1 &&
                i.RecipeCost > 1 &&
                i.Recipe.TargetItemId == i.ItemId &&
                i.CraftingProfitVsListings > 0
                && i.CraftingProfitVsSold > 0).ToList();
        viableCrafts.Sort();
        return viableCrafts;
    }

    private async Task<CraftSummaryPoco> GetSummaryForRecipe(int worldId, RecipePoco recipe)
    {
        var recipeId = recipe.Id;
        var itemId = recipe.TargetItemId;
        var recipeCost = await _recipeCostRepository.GetAsync(worldId, recipeId);
        if (recipeCost is null)
        {
            var calculatedCost = await _calc.CalculateCraftingCostForRecipe(worldId, recipeId);
            if (calculatedCost <= 1)
                throw new DataException($"Failed to calculate cost for recipe {recipeId} for1 world {worldId}");


            var newCost = new RecipeCostPoco
            {
                WorldId = worldId, RecipeId = recipeId, Cost = calculatedCost, Updated = DateTimeOffset.UtcNow
            };
            await _recipeCostRepository.Add(newCost);
            recipeCost = newCost;
        }

        var ingredients = recipe.GetActiveIngredients();

        var item = _itemRepository.Get(itemId);

        var price = _priceRepository.Get(worldId, itemId);
        var bestPrice = price?.AverageSold > 0 ? price.AverageSold : price?.AverageListingPrice;
        if (bestPrice is null or <= 0)
            throw new DataException($"Could not find price for item {itemId} for world {worldId}");
        if (recipeCost.Cost <= 0)
            throw new DataException($"Failed to find cost of recipe {recipeId}");

        var summary = new CraftSummaryPoco(
            price,
            item,
            recipeCost.Cost,
            recipe,
            ingredients
        );
        return summary;
    }
}