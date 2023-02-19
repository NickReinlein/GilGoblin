using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Crafting;

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly ICraftingCalculator _calc;
    private readonly IPriceRepository<PricePoco> _priceRepo;
    private readonly IRecipeGrocer _recipeGrocer;
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository<PricePoco> priceRepo,
        IRecipeGrocer recipeGrocer,
        IItemRepository itemRepo,
        ILogger<CraftRepository> logger
    )
    {
        _calc = calc;
        _priceRepo = priceRepo;
        _recipeGrocer = recipeGrocer;
        _itemRepo = itemRepo;
        _logger = logger;
    }

    public async Task<CraftSummaryPoco?> GetCraft(int worldID, int itemID)
    {
        var craftingCost = await _calc.CalculateCraftingCostForItem(worldID, itemID);
        var ingredients = await _recipeGrocer.BreakdownItem(itemID);
        var price = await _priceRepo.Get(worldID, itemID);
        var itemInfo = await _itemRepo.Get(itemID);
        if (craftingCost is 0 || ingredients is null || price is null || itemInfo is null)
            return null;

        return new CraftSummaryPoco(price, itemInfo, craftingCost, ingredients);
    }

    public async Task<IEnumerable<CraftSummaryPoco?>> GetBestCrafts(int worldId)
    {
        return Array.Empty<CraftSummaryPoco>();
        // var allMarketableItemIds = _priceRepo.GetAll(worldId);
    }
}
