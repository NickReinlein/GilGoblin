using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Pocos;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Repository;

public interface ICraftRepository
{
    Task<ActionResult<List<CraftSummaryPoco>>> GetBestAsync(int worldId);
    List<CraftSummaryPoco> SortByProfitability(IEnumerable<CraftSummaryPoco> crafts);

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
            _recipeProfitRepository
                .GetAll(worldId)
                .OrderByDescending(rp => rp.RecipeProfitVsSold)
                .Take(100)
                .ToList();
        if (!profits.Any())
            return new NotFoundResult();

        var recipeIds = profits.Select(i => i.RecipeId).ToList();
        var recipeCosts = _recipeCostRepository.GetMultiple(worldId, recipeIds);
        var recipes = _recipeRepository.GetMultiple(recipeIds).ToList();

        var items = _itemRepository.GetMultiple(recipes.Select(i => i.TargetItemId));
        var prices = _priceRepository.GetMultiple(worldId, recipes.Select(i => i.TargetItemId));

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
            var cached = _cache.Get((worldId, profit.RecipeId));
            if (cached is not null)
            {
                craftSummaries.Add(cached);
                continue;
            }

            var recipe = recipeList.FirstOrDefault(i => i.Id == profit.RecipeId);
            var price = priceList.FirstOrDefault(i => i.ItemId == recipe?.TargetItemId && i.WorldId == worldId);
            if (price is null)
            {
                _logger.LogError($"Failed to find price for recipe {profit.RecipeId} for world {worldId}");
                continue;
            }

            var recipeCost = recipeCostList.FirstOrDefault(i => i.RecipeId == profit.RecipeId && i.WorldId == worldId)
                ?.Cost ?? 0;
            var item = itemList.FirstOrDefault(i =>
                i.Id == recipeList.FirstOrDefault(r => r.Id == profit.RecipeId)?.TargetItemId);
            var ingredients = recipe.GetActiveIngredients();

            var summary = new CraftSummaryPoco(
                price,
                item,
                recipeCost,
                recipe,
                ingredients);
            craftSummaries.Add(summary);
            _cache.Add((summary.WorldId, summary.Recipe.Id), summary);
        }

        return Task.FromResult(craftSummaries);
    }

    public List<CraftSummaryPoco> SortByProfitability(IEnumerable<CraftSummaryPoco> crafts)
    {
        var craftsList = crafts.ToList();

        try
        {
            var viableCrafts = craftsList
                    .Where(i =>
                        (i.AverageSold > 1 || i.AverageListingPrice > 1) &&
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

    private bool ValidateWorldInput(int worldId) => _worldRepository.Get(worldId) is not null;
}