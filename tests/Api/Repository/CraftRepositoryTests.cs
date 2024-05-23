using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Pocos;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class CraftRepositoryTests
{
    private CraftRepository _repository;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IRecipeCostRepository _recipeCostRepository;
    private IRecipeProfitRepository _recipeProfitRepository;
    private IItemRepository _itemRepository;
    private IWorldRepository _worldRepository;
    private ICraftCache _cache;
    private ILogger<CraftRepository> _logger;

    [SetUp]
    public void SetUp()
    {
        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _recipeCostRepository = Substitute.For<IRecipeCostRepository>();
        _recipeProfitRepository = Substitute.For<IRecipeProfitRepository>();
        _itemRepository = Substitute.For<IItemRepository>();
        _worldRepository = Substitute.For<IWorldRepository>();
        _cache = Substitute.For<ICraftCache>();
        _logger = Substitute.For<ILogger<CraftRepository>>();
        _repository = new CraftRepository(
            _priceRepository,
            _recipeRepository,
            _recipeCostRepository,
            _recipeProfitRepository,
            _itemRepository,
            _worldRepository,
            _cache,
            _logger);
    }

    [Test]
    public async Task GivenGetBestAsync_WhenAnInvalidWorldIdIsProvided_ThenBadRequestIsReturned()
    {
        _worldRepository.Get(Arg.Any<int>()).Returns((WorldPoco)null);

        var result = await _repository.GetBestAsync(1);

        Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task GivenGetBestAsync_WhenNoProfitsAreFound_ThenNotFoundIsReturned()
    {
        _worldRepository.Get(Arg.Any<int>()).Returns(new WorldPoco());
        _recipeProfitRepository.GetAll(Arg.Any<int>()).Returns(new List<RecipeProfitPoco>());

        var result = await _repository.GetBestAsync(1);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GivenGetBestAsync_WhenProfitsAreFound_ThenOkIsReturned()
    {
        var worldId = 1;
        var recipeProfitPoco = new RecipeProfitPoco { RecipeId = 1, RecipeProfitVsSold = 100 };
        var recipePoco = new RecipePoco { Id = 1, TargetItemId = 1 };
        var pricePoco = new PricePoco { ItemId = 1, WorldId = worldId };
        var itemPoco = new ItemPoco { Id = 1 };
        _worldRepository.Get(worldId).Returns(new WorldPoco());
        _recipeProfitRepository.GetAll(worldId).Returns(new List<RecipeProfitPoco> { recipeProfitPoco });
        _recipeCostRepository.GetMultiple(worldId, Arg.Any<List<int>>()).Returns(new List<RecipeCostPoco>());
        _recipeRepository.GetMultiple(Arg.Any<List<int>>()).Returns(new List<RecipePoco> { recipePoco });
        _itemRepository.GetMultiple(Arg.Any<IEnumerable<int>>()).Returns(new List<ItemPoco> { itemPoco });
        _priceRepository.GetMultiple(worldId, Arg.Any<IEnumerable<int>>())
            .Returns(new List<PricePoco> { pricePoco });

        var result = await _repository.GetBestAsync(worldId);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<List<CraftSummaryPoco>>());
    }

    [Test]
    public void GivenSortByProfitability_WhenValidCraftsAreProvided_ThenSortedCraftsAreReturned()
    {
        var craft1 = new CraftSummaryPoco
        {
            AverageSold = 15,
            AverageListingPrice = 20,
            RecipeCost = 10,
            Recipe = new RecipePoco { Id = 1, TargetItemId = 1 },
            ItemId = 1,
            WorldId = 22,
            RecipeProfitVsListings = 10,
            RecipeProfitVsSold = 5
        };
        var craft2 = new CraftSummaryPoco
        {
            AverageSold = 50,
            AverageListingPrice = 70,
            RecipeCost = 5,
            Recipe = new RecipePoco { Id = 2, TargetItemId = 2 },
            ItemId = 2,
            WorldId = 22,
            RecipeProfitVsListings = 65,
            RecipeProfitVsSold = 45
        };
        var crafts = new List<CraftSummaryPoco> { craft2, craft1 };

        var result = _repository.SortByProfitability(crafts);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(crafts.Count));
            Assert.That(result[0], Is.EqualTo(craft2));
            Assert.That(result[1], Is.EqualTo(craft1));
        });
    }

    [Test]
    public void GivenSortByProfitability_WhenInvalidCraftsAreProvided_ThenInvalidCraftsAreFiltedOut()
    {
        var craft = new CraftSummaryPoco
        {
            AverageSold = 0,
            AverageListingPrice = 0,
            RecipeCost = 0,
            Recipe = new RecipePoco { TargetItemId = 1 },
            ItemId = 1
        };
        var crafts = new List<CraftSummaryPoco> { craft };

        var result = _repository.SortByProfitability(crafts);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenCreateSummaryAsync_WhenValidDataIsProvided_ThenValidCraftSummaryPocosAreReturned()
    {
        var worldId = 1;
        var recipeProfitPoco = new RecipeProfitPoco { RecipeId = 1, RecipeProfitVsSold = 100 };
        var recipePoco = new RecipePoco { Id = 1, TargetItemId = 1 };
        var pricePoco = new PricePoco { ItemId = 1, WorldId = worldId };
        var itemPoco = new ItemPoco { Id = 1 };

        var recipeCosts =
            new List<RecipeCostPoco> { new RecipeCostPoco { RecipeId = 1, WorldId = worldId, Cost = 10 } };
        var recipes = new List<RecipePoco> { recipePoco };
        var items = new List<ItemPoco> { itemPoco };
        var prices = new List<PricePoco> { pricePoco };
        var profits = new List<RecipeProfitPoco> { recipeProfitPoco };

        _cache.Get(Arg.Any<(int, int)>()).Returns((CraftSummaryPoco)null);

        var result = await _repository.CreateSummaryAsync(worldId, recipeCosts, recipes, items, prices, profits);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Recipe.Id, Is.EqualTo(1));
    }
}


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using GilGoblin.Api.Cache;
// using GilGoblin.Api.Pocos;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Api.Repository;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Api.Repository;
//
// public class CraftRepositoryTests : PriceDependentTests
// {
//     private CraftRepository _craftRepository;
//
//     private IPriceRepository<PricePoco> _priceRepository;
//     private IRecipeRepository _recipeRepository;
//     private IRecipeCostRepository _recipeCostRepository;
//     private IRecipeProfitRepository _recipeProfitRepository;
//     private IItemRepository _itemRepository;
//     private IWorldRepository _worldRepository;
//     private ICraftCache _cache;
//     private ILogger<CraftRepository> _logger;
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//
//         _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
//         _recipeRepository = Substitute.For<IRecipeRepository>();
//         _recipeCostRepository = Substitute.For<IRecipeCostRepository>();
//         _recipeProfitRepository = Substitute.For<IRecipeProfitRepository>();
//         _itemRepository = Substitute.For<IItemRepository>();
//         _worldRepository = Substitute.For<IWorldRepository>();
//         _cache = Substitute.For<ICraftCache>();
//         _logger = Substitute.For<ILogger<CraftRepository>>();
//
//         _craftRepository = new CraftRepository(
//             _priceRepository,
//             _recipeRepository,
//             _recipeCostRepository,
//             _recipeProfitRepository,
//             _itemRepository,
//             _worldRepository,
//             _cache,
//             _logger
//         );
//
//         var worldPoco = new WorldPoco { Id = WorldId };
//         _worldRepository.GetAll().Returns(new List<WorldPoco> { worldPoco });
//         _worldRepository.Get(WorldId).Returns(worldPoco);
//     }
//
//     [Test]
//     public async Task GivenGetBestAsync_WhenEntriesAreReturned_ThenWeCreateCraftSummaries()
//     {
//         await using var ctx = new TestGilGoblinDbContext(_options, _configuration);
//         var profits = ctx.RecipeProfit.ToList();
//         _recipeProfitRepository.GetAll(WorldId).Returns(profits);
//         _itemRepository.GetMultiple(Arg.Any<IEnumerable<int>>()).Returns(ctx.Item.ToList());
//
//         var response = await _craftRepository.GetBestAsync(WorldId);
//
//         var result = response.Result as OkObjectResult;
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Is.Not.Null);
//             Assert.That(result.StatusCode, Is.EqualTo(200));
//         });
//     }
//
//     [Test]
//     public async Task GivenGetBestAsync_WhenSuccessIsReturned_ThenTheResponseIsValid()
//     {
//         var targetItemId = SetupForSuccess();
//
//         var response = await _craftRepository.GetBestAsync(WorldId);
//
//         var result = response.Result as OkObjectResult;
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Is.Not.Null);
//             Assert.That(result.StatusCode, Is.EqualTo(200));
//             var crafts = result.Value as List<CraftSummaryPoco>;
//             Assert.That(crafts, Has.Count.GreaterThan(0));
//             var craft = crafts![0];
//             Assert.That(craft.Ingredients.ToList(), Has.Count.GreaterThan(0));
//             Assert.That(craft.AverageListingPrice, Is.GreaterThan(1));
//             Assert.That(craft.AverageSold, Is.GreaterThan(1));
//             Assert.That(craft.ItemId, Is.EqualTo(targetItemId));
//             Assert.That(craft.ItemInfo.Id, Is.EqualTo(targetItemId));
//             Assert.That(craft.Recipe.TargetItemId, Is.EqualTo(targetItemId));
//             Assert.That(craft.RecipeCost, Is.GreaterThan(1));
//             Assert.That(craft.RecipeProfitVsListings, Is.GreaterThan(1));
//             Assert.That(craft.RecipeProfitVsSold, Is.GreaterThan(1));
//             Assert.That(craft.WorldId, Is.EqualTo(WorldId));
//         });
//     }
//
//     [Test]
//     public async Task GivenGetBestAsync_WhenNoProfitsExist_ThenNotFoundIsReturned()
//     {
//         _recipeProfitRepository.GetAll(WorldId).Returns(new List<RecipeProfitPoco>());
//
//         var result = await _craftRepository.GetBestAsync(WorldId);
//
//         Assert.Multiple(async () =>
//         {
//             await _recipeCostRepository.DidNotReceive().GetAsync(WorldId, Arg.Any<int>());
//             var status = result.Result as StatusCodeResult;
//             Assert.That(status?.StatusCode, Is.EqualTo(404));
//             Assert.That(result.Value, Is.Null);
//         });
//     }
//
//     [Test]
//     public async Task GivenGetBestAsync_WhenEntriesAreReturned_ThenWeCreateSummariesForEachEntry()
//     {
//         await using var ctx = new TestGilGoblinDbContext(_options, _configuration);
//         var profits = ctx.RecipeProfit.ToList();
//         _recipeProfitRepository.GetAll(WorldId).Returns(profits);
//
//         await _craftRepository.GetBestAsync(WorldId);
//
//         foreach (var profit in profits)
//             await _recipeCostRepository.Received(1).GetAsync(WorldId, profit.RecipeId);
//     }
//
//     #region SortByProfitability
//
//     [Test]
//     public void GivenSortByProfitability_WhenTheListProvidedIsEmpty_ThenWeReturnIt()
//     {
//         var result = _craftRepository.SortByProfitability(new List<CraftSummaryPoco>());
//
//         Assert.That(result, Is.Empty);
//     }
//
//     [Test]
//     public void GivenSortByProfitability_WhenCraftsWithAllInvalidPricesAreProvided_ThenWeRemoveThemFromResults()
//     {
//         var craftSummaryPocos = GetCraftSummaryPocos();
//         craftSummaryPocos[1].AverageListingPrice = -1;
//         craftSummaryPocos[1].AverageSold = -1;
//
//         var result = _craftRepository.SortByProfitability(craftSummaryPocos);
//
//         Assert.That(result, Has.Count.EqualTo(1));
//         Assert.That(result[0].ItemId, Is.EqualTo(ItemId));
//     }
//
//     [Test]
//     public void GivenSortByProfitability_WhenValidCraftsAreProvided_ThenWeSortByProfit()
//     {
//         var craftSummaryPocos = GetCraftSummaryPocos();
//
//         var result = _craftRepository.SortByProfitability(craftSummaryPocos);
//
//         Assert.That(result, Has.Count.EqualTo(2));
//         Assert.That(result[0].RecipeProfitVsSold, Is.GreaterThanOrEqualTo(result[1].RecipeProfitVsSold));
//     }
//
//     [Test]
//     public void GivenSortByProfitability_WhenValidCraftsAreProvidedFromDifferent_ThenWeSortByWorldFirstOrDefault()
//     {
//         var otherWorldId = 999;
//         var craftSummaryPocos = GetCraftSummaryPocos();
//         craftSummaryPocos[0].WorldId = otherWorldId;
//
//         var result = _craftRepository.SortByProfitability(craftSummaryPocos);
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Has.Count.EqualTo(2));
//             Assert.That(result[0].WorldId, Is.EqualTo(WorldId));
//             Assert.That(result[0].WorldId, Is.LessThanOrEqualTo(otherWorldId));
//             Assert.That(result[1].WorldId, Is.EqualTo(otherWorldId));
//         });
//     }
//
//     [Test, Ignore("fails now?")]
//     public void GivenSortByProfitability_WhenSortingThrows_ThenTheOriginalListIsReturnedAndAnErrorLogged()
//     {
//         var throwCrafts = Substitute.For<List<CraftSummaryPoco>>();
//         throwCrafts.Where(Arg.Any<Func<CraftSummaryPoco, bool>>()).Returns(GetCraftSummaryPocos());
//         throwCrafts
//             .When(x => x.Sort())
//             .Do(_ => throw new Exception("Simulated sorting exception"));
//
//         var result = _craftRepository.SortByProfitability(throwCrafts);
//
//         Assert.That(result, Is.EqualTo(throwCrafts));
//         _logger.Received(1).LogError(Arg.Any<string>());
//     }
//
//     #endregion
//
//     private int SetupForSuccess()
//     {
//         using var ctx = new TestGilGoblinDbContext(_options, _configuration);
//         var recipes = ctx.Recipe.Where(r => r.Id == RecipeId).ToList();
//         var item = ctx.Item.FirstOrDefault(i => i.Id == ItemId);
//         var price = ctx.Price.FirstOrDefault(p => p.WorldId == WorldId && p.ItemId == ItemId);
//         var targetItemIdForRecipe = recipes.FirstOrDefault(r => r.Id == RecipeId)?.TargetItemId ?? 0;
//         var price2 = ctx.Price.FirstOrDefault(p => p.WorldId == WorldId && p.ItemId == targetItemIdForRecipe);
//         var profits = ctx.RecipeProfit.Where(rp => rp.WorldId == WorldId).ToList();
//         var item2 = ctx.Item.FirstOrDefault(i => i.Id == targetItemIdForRecipe);
//         var recipeCost = ctx.RecipeCost.FirstOrDefault(p => p.WorldId == WorldId && p.RecipeId == RecipeId);
//         _recipeProfitRepository.GetAll(WorldId).Returns(profits);
//         _recipeCostRepository.GetAsync(WorldId, RecipeId).Returns(recipeCost);
//         _recipeRepository.GetAll().Returns(recipes);
//         _recipeRepository.Get(RecipeId).Returns(recipes.FirstOrDefault(r => r.Id == RecipeId));
//         _recipeRepository.GetMultiple(Arg.Any<IEnumerable<int>>()).Returns(recipes);
//         _priceRepository.Get(WorldId, ItemId).Returns(price);
//         _priceRepository.Get(WorldId, targetItemIdForRecipe).Returns(price2);
//         _itemRepository.Get(ItemId).Returns(item);
//         _itemRepository.Get(targetItemIdForRecipe).Returns(item2);
//         return targetItemIdForRecipe;
//     }
//
//     private static List<CraftSummaryPoco> GetCraftSummaryPocos()
//     {
//         return new List<CraftSummaryPoco>
//         {
//             new()
//             {
//                 WorldId = WorldId,
//                 ItemId = ItemId,
//                 AverageSold = 100,
//                 AverageListingPrice = 101,
//                 RecipeCost = 80,
//                 Recipe = new RecipePoco { Id = RecipeId, TargetItemId = ItemId },
//                 RecipeProfitVsListings = 21,
//                 RecipeProfitVsSold = 20
//             },
//             new()
//             {
//                 WorldId = WorldId,
//                 ItemId = ItemId2,
//                 AverageSold = 200,
//                 AverageListingPrice = 202,
//                 RecipeCost = 111,
//                 Recipe = new RecipePoco { Id = RecipeId2, TargetItemId = ItemId2 },
//                 RecipeProfitVsListings = 91,
//                 RecipeProfitVsSold = 89
//             }
//         };
//     }
// }