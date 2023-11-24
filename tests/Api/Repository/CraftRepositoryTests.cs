using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class CraftRepositoryTests : PriceDependentTests
{
    private CraftRepository _craftRepository;

    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IRecipeCostRepository _recipeCostRepository;
    private IRecipeProfitRepository _recipeProfitRepository;
    private IItemRepository _itemRepository;
    private ICraftCache _cache;
    private ILogger<CraftRepository> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _recipeCostRepository = Substitute.For<IRecipeCostRepository>();
        _recipeProfitRepository = Substitute.For<IRecipeProfitRepository>();
        _itemRepository = Substitute.For<IItemRepository>();
        _cache = Substitute.For<ICraftCache>();
        _logger = Substitute.For<ILogger<CraftRepository>>();

        _craftRepository = new CraftRepository(
            _priceRepository,
            _recipeRepository,
            _recipeCostRepository,
            _recipeProfitRepository,
            _itemRepository,
            _cache,
            _logger
        );
    }

    [Test]
    public async Task GivenGetBestCraftsAsync_WhenEntriesAreReturned_ThenWeCreateCraftSummaries()
    {
        await using var ctx = new TestGilGoblinDbContext(_options, _configuration);
        var profits = ctx.RecipeProfit.ToList();
        _recipeProfitRepository.GetAll(WorldId).Returns(profits);

        await _craftRepository.GetBestCraftsAsync(WorldId);

        foreach (var profit in profits)
            await _recipeCostRepository.Received().GetAsync(WorldId, profit.RecipeId);
    }

    [Test]
    public async Task GivenGetBestCraftsAsync_WhenEntriesAreReturned_ThenTheSummaryReturnedIsValid()
    {
        var targetItemId = SetupForSuccess();

        var results = await _craftRepository.GetBestCraftsAsync(WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Empty);
            var result = results.FirstOrDefault(cs => cs.ItemId == targetItemId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Ingredients.ToList(), Has.Count.GreaterThan(0));
            Assert.That(result.AverageListingPrice, Is.GreaterThan(1));
            Assert.That(result.AverageSold, Is.GreaterThan(1));
            Assert.That(result.ItemId, Is.EqualTo(targetItemId));
            Assert.That(result.ItemInfo.Id, Is.EqualTo(targetItemId));
            Assert.That(result.Recipe.TargetItemId, Is.EqualTo(targetItemId));
            Assert.That(result.RecipeCost, Is.GreaterThan(1));
            Assert.That(result.RecipeProfitVsListings, Is.GreaterThan(1));
            Assert.That(result.RecipeProfitVsSold, Is.GreaterThan(1));
            Assert.That(result.WorldId, Is.EqualTo(WorldId));
        });
    }

    [Test]
    public async Task GivenGetBestCraftsAsync_WhenNothingIsReturned_ThenWeStopAndReturnAnEmptyList()
    {
        _recipeProfitRepository.GetAll(WorldId).Returns(new List<RecipeProfitPoco>());

        var result = await _craftRepository.GetBestCraftsAsync(WorldId);

        await _recipeCostRepository.DidNotReceive().GetAsync(WorldId, Arg.Any<int>());
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenGetBestCraftsAsync_WhenEntriesAreReturned_ThenWeCreateSummariesForEachEntry()
    {
        await using var ctx = new TestGilGoblinDbContext(_options, _configuration);
        var profits = ctx.RecipeProfit.ToList();
        _recipeProfitRepository.GetAll(WorldId).Returns(profits);

        await _craftRepository.GetBestCraftsAsync(WorldId);

        foreach (var profit in profits)
            await _recipeCostRepository.Received().GetAsync(WorldId, profit.RecipeId);
    }

    [Test]
    public void GivenSortByProfitability_WhenTheListProvidedIsEmpty_ThenWeReturnIt()
    {
        var result = _craftRepository.SortByProfitability(new List<CraftSummaryPoco>());

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GivenSortByProfitability_WhenInvalidCraftsAreProvided_ThenWeSanitizeThemFromResults()
    {
        var craftSummaryPocos = GetCraftSummaryPocos();
        craftSummaryPocos[1].AverageListingPrice = 0;

        var result = _craftRepository.SortByProfitability(craftSummaryPocos);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ItemId, Is.EqualTo(ItemId));
    }

    [Test]
    public void GivenSortByProfitability_WhenValidCraftsAreProvided_ThenWeSortByProfit()
    {
        var craftSummaryPocos = GetCraftSummaryPocos();

        var result = _craftRepository.SortByProfitability(craftSummaryPocos);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].RecipeProfitVsSold, Is.GreaterThanOrEqualTo(result[1].RecipeProfitVsSold));
    }

    [Test]
    public void GivenSortByProfitability_WhenValidCraftsAreProvidedFromDifferent_ThenWeSortByWorldFirstOrDefault()
    {
        var otherWorldId = 999;
        var craftSummaryPocos = GetCraftSummaryPocos();
        craftSummaryPocos[0].WorldId = otherWorldId;

        var result = _craftRepository.SortByProfitability(craftSummaryPocos);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].WorldId, Is.EqualTo(WorldId));
            Assert.That(result[0].WorldId, Is.LessThanOrEqualTo(otherWorldId));
            Assert.That(result[1].WorldId, Is.EqualTo(otherWorldId));
        });
    }

    private int SetupForSuccess()
    {
        using var ctx = new TestGilGoblinDbContext(_options, _configuration);
        var recipes = ctx.Recipe.Where(r => r.Id == RecipeId).ToList();
        var item = ctx.Item.FirstOrDefault(i => i.Id == ItemId);
        var price = ctx.Price.FirstOrDefault(p => p.WorldId == WorldId && p.ItemId == ItemId);
        var targetItemIdForRecipe = recipes.First(r => r.Id == RecipeId).TargetItemId;
        var price2 = ctx.Price.FirstOrDefault(p => p.WorldId == WorldId && p.ItemId == targetItemIdForRecipe);
        var profits = ctx.RecipeProfit.Where(rp => rp.RecipeId == RecipeId).ToList();
        var item2 = ctx.Item.FirstOrDefault(i => i.Id == targetItemIdForRecipe);
        var recipeCost = ctx.RecipeCost.FirstOrDefault(p => p.WorldId == WorldId && p.RecipeId == RecipeId);
        _recipeProfitRepository.GetAll(WorldId).Returns(profits);
        _recipeCostRepository.GetAsync(WorldId, RecipeId).Returns(recipeCost);
        _recipeRepository.GetAll().Returns(recipes);
        _recipeRepository.Get(RecipeId).Returns(recipes.FirstOrDefault(r => r.Id == RecipeId));
        _priceRepository.Get(WorldId, ItemId).Returns(price);
        _priceRepository.Get(WorldId, targetItemIdForRecipe).Returns(price2);
        _itemRepository.Get(ItemId).Returns(item);
        _itemRepository.Get(targetItemIdForRecipe).Returns(item2);
        return targetItemIdForRecipe;
    }


    private static List<CraftSummaryPoco> GetCraftSummaryPocos()
    {
        return new List<CraftSummaryPoco>()
        {
            new()
            {
                WorldId = WorldId,
                ItemId = ItemId,
                AverageSold = 100,
                AverageListingPrice = 101,
                RecipeCost = 80,
                Recipe = new RecipePoco { Id = RecipeId, TargetItemId = ItemId },
                RecipeProfitVsListings = 21,
                RecipeProfitVsSold = 20
            },
            new()
            {
                WorldId = WorldId,
                ItemId = ItemId2,
                AverageSold = 200,
                AverageListingPrice = 202,
                RecipeCost = 111,
                Recipe = new RecipePoco { Id = RecipeId2, TargetItemId = ItemId2 },
                RecipeProfitVsListings = 91,
                RecipeProfitVsSold = 89
            }
        };
    }
}