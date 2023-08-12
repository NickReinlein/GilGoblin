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
        var (recipeId, craftingCost) = await _calc.CalculateCraftingCostForItem(worldID, itemID);
        if (recipeId is 0)
            return null;

        var recipe = _recipeRepository.Get(recipeId);
        if (recipe is null)
            return null;
        var ingredients = recipe.GetActiveIngredients();

        var price = _priceRepository.Get(worldID, itemID);
        var itemInfo = _itemRepository.Get(itemID);

        return new CraftSummaryPoco(price, itemInfo, craftingCost, recipe, ingredients);
    }

    public IEnumerable<CraftSummaryPoco> GetBestCrafts(int worldID)
    {
        return Array.Empty<CraftSummaryPoco>();
    }
}
