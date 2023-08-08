using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Extensions;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Crafting;

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly ICraftingCalculator _calc;
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeGrocer _recipeGrocer;
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IRecipeGrocer recipeGrocer,
        IItemRepository itemRepository,
        ILogger<CraftRepository> logger
    )
    {
        _calc = calc;
        _priceRepository = priceRepo;
        _recipeGrocer = recipeGrocer;
        _recipeRepository = recipeRepository;
        _itemRepo = itemRepository;
        _logger = logger;
    }

    public async Task<CraftSummaryPoco?> GetBestCraft(int worldID, int itemID)
    {
        _logger.LogInformation("Getting craft {ItemID} for world {WorldID}", itemID, worldID);
        var (recipeId, craftingCost) = await _calc.CalculateCraftingCostForItem(worldID, itemID);
        var recipe = _recipeRepository.Get(recipeId);
        var ingredients = recipe.GetActiveIngredients();
        var price = _priceRepository.Get(worldID, itemID);
        var itemInfo = _itemRepo.Get(itemID);
        if (craftingCost is 0 || ingredients is null || price is null || itemInfo is null)
            return null;

        return new CraftSummaryPoco(price, itemInfo, craftingCost, ingredients);
    }

    public IEnumerable<CraftSummaryPoco> GetBestWorldCrafts(int worldID)
    {
        _logger.LogInformation("Getting best crafts for world {WorldID}", worldID);
        return Array.Empty<CraftSummaryPoco>();
        // var allMarketableItemIds = _priceRepo.GetAll(worldId);
    }
}
