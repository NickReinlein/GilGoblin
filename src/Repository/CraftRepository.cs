using GilGoblin.Crafting;
using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly ICraftingCalculator _calc;
    private readonly IPriceRepository _priceRepo;
    private readonly IRecipeGrocer _recipeGrocer;
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository priceRepo,
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
        var craftingCost = _calc.CalculateCraftingCostForItem(worldID, itemID);
        var ingredients = _recipeGrocer.BreakdownItem(itemID);
        var price = _priceRepo.Get(worldID, itemID);
        var itemInfo = await _itemRepo.Get(itemID);
        return new CraftSummaryPoco(price, itemInfo, craftingCost, ingredients);
    }

    public Task<IEnumerable<CraftSummaryPoco?>> GetBestCrafts(int worldId)
    {
        // return Enumerable.Range(1, 5).Select(index => GetCraft(worldId, index)).ToArray();
        return null;
    }
}
