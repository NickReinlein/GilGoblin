using System;
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

namespace GilGoblin.Tests.Api.Crafting;

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

    private static int WorldId => 1;

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
        _recipeProfitRepository.GetAllAsync(Arg.Any<int>()).Returns(new List<RecipeProfitPoco>());

        var result = await _repository.GetBestAsync(1);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GivenGetBestAsync_WhenProfitsAreFound_ThenProfitsAreCachedAndOkIsReturned()
    {
        var recipeProfitPoco = new RecipeProfitPoco(1, WorldId, true, 111, DateTimeOffset.UtcNow);
        var recipeCosts = new List<RecipeCostPoco> { new(1, WorldId, true, 100, DateTimeOffset.UtcNow) };
        var recipePoco = new RecipePoco { Id = 1, TargetItemId = 1 };
        var pricePoco = new PricePoco(1, WorldId, true);
        var itemPoco = new ItemPoco { Id = 1 };
        _worldRepository.Get(WorldId).Returns(new WorldPoco());
        _recipeProfitRepository.GetAllAsync(WorldId).Returns([recipeProfitPoco]);
        _recipeCostRepository.GetMultipleAsync(WorldId, Arg.Any<List<int>>()).Returns(recipeCosts);
        _recipeRepository.GetMultiple(Arg.Any<List<int>>()).Returns([recipePoco]);
        _itemRepository.GetMultiple(Arg.Any<IEnumerable<int>>()).Returns([itemPoco]);
        _priceRepository.GetMultiple(WorldId, Arg.Any<IEnumerable<int>>(), Arg.Any<bool>())
            .Returns([pricePoco]);

        var result = await _repository.GetBestAsync(WorldId);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<List<CraftSummaryPoco>>());
        _cache.Received(1).Add(new TripleKey(WorldId, 1, true), Arg.Any<CraftSummaryPoco>());
    }

    [Test]
    public async Task GivenCreateSummaryAsync_WhenValidDataIsProvided_ThenValidCraftSummaryPocosAreReturned()
    {
        var recipeProfitPoco = new RecipeProfitPoco(1, WorldId, true, 111, DateTimeOffset.UtcNow);
        var recipePoco = new RecipePoco { Id = 1, TargetItemId = 1 };
        var pricePoco = new PricePoco(1, WorldId, true);
        var itemPoco = new ItemPoco { Id = 1 };
        var recipeCosts = new List<RecipeCostPoco> { new(1, WorldId, true, 100, DateTimeOffset.UtcNow) };
        var recipes = new List<RecipePoco> { recipePoco };
        var items = new List<ItemPoco> { itemPoco };
        var prices = new List<PricePoco> { pricePoco };
        var profits = new List<RecipeProfitPoco> { recipeProfitPoco };
        _cache.Get(Arg.Any<TripleKey>()).Returns((CraftSummaryPoco)null!);

        var result = await _repository.CreateSummaryAsync(WorldId, recipeCosts, recipes, items, prices, profits);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Recipe!.Id, Is.EqualTo(1));
    }
}