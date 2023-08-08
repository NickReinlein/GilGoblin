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
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IItemRepository itemRepository,
        ILogger<CraftRepository> logger
    )
    {
        _calc = calc;
        _priceRepository = priceRepo;
        _recipeRepository = recipeRepository;
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<CraftSummaryPoco?> GetBestCraft(int worldID, int itemID)
    {
        _logger.LogInformation("Getting craft {ItemID} for world {WorldID}", itemID, worldID);
        var (recipeId, craftingCost) = await _calc.CalculateCraftingCostForItem(worldID, itemID);
        var recipe = _recipeRepository.Get(recipeId);
        var ingredients = recipe.GetActiveIngredients();
        var price = _priceRepository.Get(worldID, itemID);
        var itemInfo = _itemRepository.Get(itemID);
        if (craftingCost is 0 || ingredients is null || price is null || itemInfo is null)
            return null;

        return new CraftSummaryPoco(price, itemInfo, craftingCost, ingredients);
    }

    public IEnumerable<CraftSummaryPoco> GetBestCrafts(int worldID)
    {
        _logger.LogInformation("Getting best crafts for world {WorldID}", worldID);
        return Array.Empty<CraftSummaryPoco>();
    }
}
