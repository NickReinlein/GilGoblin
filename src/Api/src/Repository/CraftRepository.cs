using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Pocos;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Repository;

public interface ICraftRepository
{
    Task<ActionResult<List<CraftSummaryPoco>>> GetBestAsync(int worldId);

    Task<List<CraftSummaryPoco>> CreateSummaryAsync(
        int worldId,
        IEnumerable<RecipeCostPoco> recipeCosts,
        IEnumerable<RecipePoco> recipes,
        IEnumerable<ItemPoco> items,
        IEnumerable<PricePoco> prices,
        IEnumerable<RecipeProfitPoco> profits);
}

public class CraftRepository : ICraftRepository
{
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCostRepository _recipeCostRepository;
    private readonly IRecipeProfitRepository _recipeProfitRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWorldRepository _worldRepository;
    private readonly ICraftCache _cache;
    private readonly ILogger<CraftRepository> _logger;

    public CraftRepository(
        IPriceRepository<PricePoco> priceRepo,
        IRecipeRepository recipeRepository,
        IRecipeCostRepository recipeCostRepository,
        IRecipeProfitRepository recipeProfitRepository,
        IItemRepository itemRepository,
        IWorldRepository worldRepository,
        ICraftCache cache,
        ILogger<CraftRepository> logger)
    {
        _priceRepository = priceRepo;
        _recipeRepository = recipeRepository;
        _recipeCostRepository = recipeCostRepository;
        _recipeProfitRepository = recipeProfitRepository;
        _itemRepository = itemRepository;
        _worldRepository = worldRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ActionResult<List<CraftSummaryPoco>>> GetBestAsync(int worldId)
    {
        if (!ValidateWorldInput(worldId))
            return new BadRequestResult();

        var profits =
            (await _recipeProfitRepository.GetAllAsync(worldId))
            .OrderByDescending(rp => rp.Amount)
            .Take(100)
            .ToList();
        if (!profits.Any())
            return new NotFoundResult();

        var recipeIds = profits.Select(i => i.RecipeId).ToList();
        var recipeCosts = await _recipeCostRepository.GetMultipleAsync(worldId, recipeIds);
        var recipes = _recipeRepository.GetMultiple(recipeIds).ToList();

        var items = _itemRepository.GetMultiple(recipes.Select(i => i.TargetItemId));
        var nqPrices = _priceRepository.GetMultiple(worldId, recipes.Select(i => i.TargetItemId), false);
        var hqPrices = _priceRepository.GetMultiple(worldId, recipes.Select(i => i.TargetItemId), true);
        var prices = nqPrices.Concat(hqPrices).ToList();

        var crafts = await CreateSummaryAsync(worldId, recipeCosts, recipes, items, prices, profits);
        return crafts.Any() ? new OkObjectResult(crafts) : new NotFoundResult();
    }

    public Task<List<CraftSummaryPoco>> CreateSummaryAsync(
        int worldId,
        IEnumerable<RecipeCostPoco> recipeCosts,
        IEnumerable<RecipePoco> recipes,
        IEnumerable<ItemPoco> items,
        IEnumerable<PricePoco> prices,
        IEnumerable<RecipeProfitPoco> profits)
    {
        var craftSummaries = new List<CraftSummaryPoco>();
        var recipeCostList = recipeCosts.ToList();
        var recipeList = recipes.ToList();
        var itemList = items.ToList();
        var priceList = prices.ToList();
        var profitList = profits.ToList();

        foreach (var profit in profitList)
        {
            var cached = _cache.Get(new TripleKey(worldId, profit.RecipeId, profit.IsHq));
            if (cached is not null)
            {
                craftSummaries.Add(cached);
                continue;
            }

            var recipe = recipeList.FirstOrDefault(i => i.Id == profit.RecipeId);
            var price = priceList.FirstOrDefault(i =>
                i.ItemId == recipe?.TargetItemId &&
                i.WorldId == worldId &&
                i.IsHq == profit.IsHq);
            if (price is null)
            {
                _logger.LogError($"Failed to find price for recipe {profit.RecipeId} for world {worldId}");
                continue;
            }

            var recipeCost = recipeCostList.FirstOrDefault(i =>
                i.RecipeId == profit.RecipeId &&
                i.IsHq == profit.IsHq &&
                i.WorldId == worldId)?.Amount;
            if (recipeCost is null)
            {
                _logger.LogError(
                    "Failed to find recipe cost or item to create craft summary for recipe {RecipeId}, world {WorldId}, hq: {IsHq}",
                    profit.RecipeId,
                    worldId,
                    profit.IsHq);
                continue;
            }


            var item = itemList.FirstOrDefault(i =>
                i.Id == recipeList.FirstOrDefault(r => r.Id == profit.RecipeId)?.TargetItemId);
            if (item is null)
            {
                _logger.LogError(
                    "Failed to find target item {ItemId} to create craft summary for recipe {RecipeId}, world {WorldId}, hq: {IsHq}",
                    recipe?.TargetItemId ?? 0,
                    profit.RecipeId,
                    worldId,
                    profit.IsHq);
                continue;
            }

            var salePrice = (int)price.GetBestPriceCost();
            var craftingCost = recipeCost.Value;
            var craftingProfit = salePrice - craftingCost;
            var summary = new CraftSummaryPoco
            {
                RecipeId = profit.RecipeId,
                WorldId = profit.WorldId,
                IsHq = profit.IsHq,
                ItemId = item.Id,
                ItemInfo = item,
                Recipe = recipe,
                SalePrice = salePrice,
                CraftingCost = craftingCost,
                Profit = craftingProfit,
                Updated = profit.LastUpdated
            };
            craftSummaries.Add(summary);
            _cache.Add(new TripleKey(summary.WorldId, summary.RecipeId, summary.IsHq), summary);
        }

        return Task.FromResult(craftSummaries);
    }

    private bool ValidateWorldInput(int worldId) => _worldRepository.Get(worldId) is not null;
}