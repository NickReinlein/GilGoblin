using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class CraftRepositoryTests
{
    private CraftRepository _craftRepository;

    private ICraftingCalculator _calc;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IRecipeCostRepository _recipeCostRepository;
    private IItemRepository _itemRepository;
    private ICraftCache _cache;
    private ILogger<CraftRepository> _logger;
    private readonly int _worldId = 22;
    private readonly int _itemId = 6400;
    private readonly int _recipeId = 444;
    private readonly int _recipeCost = 777;
    private readonly string _itemName = "Excalibur";

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenOtherRepositoriesAreCalled()
    {
        await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        await _calc.Received().CalculateCraftingCostForItem(_worldId, _itemId);
        _recipeRepository.Received().Get(_recipeId);
        _priceRepository.Received().Get(_worldId, _itemId);
        _itemRepository.Received().Get(_itemId);
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenASummaryIsReturned()
    {
        var result = await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RecipeCost, Is.EqualTo(_recipeCost));
            Assert.That(result.WorldId, Is.EqualTo(_worldId));
            Assert.That(result.ItemId, Is.EqualTo(_itemId));
            Assert.That(result.Recipe.Id, Is.EqualTo(_recipeId));
            Assert.That(result.Name, Is.EqualTo(_itemName));
        });
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsInvalid_ThenNullIsReturned()
    {
        _calc.CalculateCraftingCostForItem(_worldId, _itemId).Returns((0, 0));

        var result = await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        Assert.That(result, Is.Null);
    }

    // [Test]
    // public async Task GivenGetBestCrafts_WhenThereAreMultipleResults_ThenEachOneIsProcessed()
    // {
    //     var crafts = GetCrafts();
    //     var secondItemId = crafts[1].TargetItemId;
    //     var secondRecipeId = crafts[1].Id;
    //     _calc.CalculateCraftingCostForItem(_worldId, secondItemId).Returns((secondRecipeId, _recipeCost + 100));
    //     _priceRepository.Get(_worldId, secondItemId)
    //         .Returns(new PricePoco { WorldId = _worldId, ItemId = secondItemId });
    //     _recipeRepository.GetAll().Returns(crafts);
    //     // _recipeRepository.Get(secondRecipeId)
    //     //     .Returns(new RecipePoco { Id = secondRecipeId, TargetItemId = secondItemId });
    //     _itemRepository.Get(secondItemId).Returns(new ItemPoco { Id = secondItemId, Name = _itemName });
    //
    //     var result = await _craftRepository.GetBestCraftsForWorld(_worldId);
    //
    //     Assert.That(result.Count, Is.EqualTo(2));
    //     await _calc.Received(1).CalculateCraftingCostForItem(_worldId, _itemId);
    //     await _calc.Received(1).CalculateCraftingCostForItem(_worldId, secondItemId);
    //     _recipeRepository.Received(1).Get(_recipeId);
    //     _recipeRepository.Received(1).Get(secondRecipeId);
    //     _priceRepository.Received(1).Get(_worldId, _itemId);
    //     _priceRepository.Received(1).Get(_worldId, secondItemId);
    //     _itemRepository.Received(1).Get(_itemId);
    //     _itemRepository.Received(1).Get(secondItemId);
    // }
    //
    // [Test]
    // public async Task GivenGetBestCrafts_WhenARecipeThrowsAnExeption_ThenAnErrorIsLoggedAndOthersRecipesAreReturned()
    // {
    //     var crafts = GetCrafts();
    //     var goodRecipe = crafts[0];
    //     var badRecipe = crafts[1];
    //     _calc.CalculateCraftingCostForItem(_worldId, goodRecipe.TargetItemId).Returns((goodRecipe.Id, 600));
    //     _calc.CalculateCraftingCostForItem(_worldId, badRecipe.TargetItemId).Throws<ArithmeticException>();
    //
    //     var result = await _craftRepository.GetBestCraftsForWorld(_worldId);
    //
    //     // await _calc.Received(1).CalculateCraftingCostForItem(_worldId, goodRecipe.TargetItemId);
    //     // await _calc.Received(1).CalculateCraftingCostForItem(_worldId, badRecipe.TargetItemId);
    //     Assert.That(result.Count, Is.EqualTo(1));
    //     _recipeRepository.Received(1).Get(goodRecipe.Id);
    //     _logger
    //         .DidNotReceive()
    //         .LogError($"Failed to calculate best craft for item {goodRecipe.TargetItemId} in world {_worldId}");
    //     _logger
    //         .Received(1)
    //         .LogError($"Failed to calculate best craft for item {badRecipe.TargetItemId} in world {_worldId}");
    // }

    [Test]
    public async Task GivenAGetBestCraft_WhenTheIdIsValidAndUncached_ThenWeCacheTheNewEntry()
    {
        _cache.Get((_worldId, _itemId)).Returns((CraftSummaryPoco)null);

        _ = await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        _cache.Received(1).Get((_worldId, _itemId));
        _cache.Received(1).Add((_worldId, _itemId),
            Arg.Is<CraftSummaryPoco>(s => s.ItemId == _itemId && s.WorldId == _worldId));
    }


    [Test]
    public async Task GivenAGetBestCraft_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        var summary = new CraftSummaryPoco { WorldId = _worldId, ItemId = _itemId };
        _cache.Get((_worldId, _itemId)).Returns(summary);

        _ = await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        _cache.Received(1).Get((_worldId, _itemId));
        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<CraftSummaryPoco>());
    }

    [Test]
    public async Task GivenAGetBestCraft_WhenTheIdIsValidButNoRecipesExist_ThenWeReturnNull()
    {
        _recipeRepository.Get(_recipeId).Returns((RecipePoco)null);

        var result = await _craftRepository.GetBestCraftForItem(_worldId, _itemId);

        Assert.That(result, Is.Null);
        _cache.Received().Get((_worldId, _itemId));
        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<CraftSummaryPoco>());
    }

    [SetUp]
    public void SetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _calc.CalculateCraftingCostForItem(_worldId, _itemId).Returns((_recipeId, _recipeCost));

        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _priceRepository
            .Get(_worldId, _itemId)
            .Returns(new PricePoco { WorldId = _worldId, ItemId = _itemId });

        _recipeRepository = Substitute.For<IRecipeRepository>();
        _recipeRepository.Get(_recipeId).Returns(new RecipePoco { Id = _recipeId, TargetItemId = _itemId });
        _recipeRepository.GetAll().Returns(GetCrafts());

        _recipeCostRepository = Substitute.For<IRecipeCostRepository>();
        _itemRepository = Substitute.For<IItemRepository>();
        _itemRepository.Get(_itemId).Returns(new ItemPoco { Id = _itemId, Name = _itemName });

        _cache = Substitute.For<ICraftCache>();
        _cache
            .Get((_worldId, _itemId))
            .Returns(null, new CraftSummaryPoco() { ItemId = _itemId, Name = _itemName });

        _logger = Substitute.For<ILogger<CraftRepository>>();

        _craftRepository = new CraftRepository(
            _calc,
            _priceRepository,
            _recipeRepository,
            _recipeCostRepository,
            _itemRepository,
            _cache,
            _logger
        );
    }

    private List<RecipePoco> GetCrafts() => new()
    {
        new() { Id = _recipeId, TargetItemId = _itemId }, new() { Id = _recipeId + 1, TargetItemId = _itemId + 1 }
    };
}