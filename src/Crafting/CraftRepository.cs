using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using GilGoblin.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Crafting;

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    private readonly ICraftingCalculator _calc;
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICraftCache _cache;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        ICraftingCalculator calc,
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IItemRepository itemRepository,
        ICraftCache cache,
        ILogger<CraftRepository> logger
    )
    {
        _calc = calc;
        _priceRepository = priceRepo;
        _recipeRepository = recipeRepository;
        _itemRepository = itemRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<CraftSummaryPoco?> GetBestCraft(int worldId, int itemId)
    {
        var cached = _cache.Get((worldId, itemId));
        if (cached is not null)
            return cached;

        var (recipeId, craftingCost) = await _calc.CalculateCraftingCostForItem(worldId, itemId);
        if (recipeId is < 1 || craftingCost.IsErrorCost())
            return null;

        var recipe = _recipeRepository.Get(recipeId);
        if (recipe is null)
            return null;
        var ingredients = recipe.GetActiveIngredients();

        var price = _priceRepository.Get(worldId, itemId);
        var itemInfo = _itemRepository.Get(itemId);

        var craftSummaryPoco = new CraftSummaryPoco(
            price,
            itemInfo,
            craftingCost,
            recipe,
            ingredients
        );
        _cache.Add((worldId, itemId), craftSummaryPoco);
        return craftSummaryPoco;
    }

    public async Task<IEnumerable<CraftSummaryPoco>> GetBestCrafts(int worldId)
    {
        var allItemsWithRecipes = _recipeRepository
            .GetAll()
            .Select(r => r.TargetItemId)
            .Distinct()
            .ToList();

        var bestCraftPerItem = new List<CraftSummaryPoco>();
        foreach (var itemId in allItemsWithRecipes)
        {
            try
            {
                var result = await GetBestCraft(worldId, itemId);
                if (result is not null)
                    bestCraftPerItem.Add(result);
            }
            catch
            {
                _logger.LogError(
                    $"Failed to calculate best craft for item {itemId} in world {worldId}"
                );
            }
        }

        // Sort by profit decreasingly vs previouslySold
        bestCraftPerItem.Sort();
        return bestCraftPerItem;
    }
}
