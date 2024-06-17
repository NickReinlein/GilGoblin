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
        _worldRepository.Get(Arg.Any<int>()).Returns((WorldPoco)null!);

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

        _cache.Get(Arg.Any<(int, int)>()).Returns((CraftSummaryPoco)null!);

        var result = await _repository.CreateSummaryAsync(worldId, recipeCosts, recipes, items, prices, profits);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Recipe.Id, Is.EqualTo(1));
    }
}