using GilGoblin.Cache;
using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class CraftRepositoryTests
{
    private CraftRepository _craftRepository;

    private ICraftingCalculator _calc;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IItemRepository _itemRepository;
    private ICraftCache _cache;
    private ILogger<CraftRepository> _logger;
    private readonly int _worldID = 22;
    private readonly int _itemID = 6400;
    private readonly int _recipeID = 444;
    private readonly int _craftingCost = 777;
    private readonly string _itemName = "Excalibur";

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenOtherRepositoriesAreCalled()
    {
        await _craftRepository.GetBestCraft(_worldID, _itemID);

        await _calc.Received().CalculateCraftingCostForItem(_worldID, _itemID);
        _recipeRepository.Received().Get(_recipeID);
        _priceRepository.Received().Get(_worldID, _itemID);
        _itemRepository.Received().Get(_itemID);
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenASummaryIsReturned()
    {
        var result = await _craftRepository.GetBestCraft(_worldID, _itemID);

        Assert.Multiple(() =>
        {
            Assert.That(result.CraftingCost, Is.EqualTo(_craftingCost));
            Assert.That(result.WorldID, Is.EqualTo(_worldID));
            Assert.That(result.ItemID, Is.EqualTo(_itemID));
            Assert.That(result.Recipe.ID, Is.EqualTo(_recipeID));
            Assert.That(result.Name, Is.EqualTo(_itemName));
        });
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsInvalid_ThenNullIsReturned()
    {
        _calc.CalculateCraftingCostForItem(_worldID, _itemID).Returns((0, 0));

        var result = await _craftRepository.GetBestCraft(_worldID, _itemID);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenGetBestCrafts_WhenThereAreMultipleResults_ThenEachOneIsProcessed()
    {
        var crafts = GetCrafts();
        var secondItemID = crafts[1].TargetItemID;
        var secondRecipeID = crafts[1].ID;
        _calc.CalculateCraftingCostForItem(_worldID, secondItemID).Returns((secondRecipeID, _craftingCost + 100));
        _priceRepository.Get(_worldID, secondItemID).Returns(new PricePoco { WorldID = _worldID, ItemID = secondItemID });
        _recipeRepository.Get(secondRecipeID).Returns(new RecipePoco { ID = secondRecipeID, TargetItemID = secondItemID });
        _itemRepository.Get(secondItemID).Returns(new ItemInfoPoco { ID = secondItemID, Name = _itemName });

        var result = await _craftRepository.GetBestCrafts(_worldID);

        Assert.That(result.Count, Is.EqualTo(2));
        await _calc.Received(1).CalculateCraftingCostForItem(_worldID, _itemID);
        await _calc.Received(1).CalculateCraftingCostForItem(_worldID, secondItemID);
        _recipeRepository.Received(1).Get(_recipeID);
        _recipeRepository.Received(1).Get(secondRecipeID);
        _priceRepository.Received(1).Get(_worldID, _itemID);
        _priceRepository.Received(1).Get(_worldID, secondItemID);
        _itemRepository.Received(1).Get(_itemID);
        _itemRepository.Received(1).Get(secondItemID);
    }

    [Test]
    public async Task GivenGetBestCrafts_WhenARecipeThrowsAnExeption_ThenAnErrorIsLoggedAndOthersRecipesAreReturned()
    {
        var crafts = GetCrafts();
        var goodRecipe = crafts[0];
        var badRecipe = crafts[1];
        _calc.CalculateCraftingCostForItem(_worldID, badRecipe.TargetItemID).Throws<ArithmeticException>();

        var result = await _craftRepository.GetBestCrafts(_worldID);

        await _calc.Received(1).CalculateCraftingCostForItem(_worldID, goodRecipe.TargetItemID);
        await _calc.Received(1).CalculateCraftingCostForItem(_worldID, badRecipe.TargetItemID);
        Assert.That(result.Count, Is.EqualTo(1));
        _recipeRepository.Received(1).Get(goodRecipe.ID);
        _logger
           .DidNotReceive()
           .LogError($"Failed to calculate best craft for item {goodRecipe.TargetItemID} in world {_worldID}");
        _logger
            .Received(1)
            .LogError($"Failed to calculate best craft for item {badRecipe.TargetItemID} in world {_worldID}");
    }

    [Test]
    public async Task GivenAGetBestCraft_WhenTheIDIsValidAndUncached_ThenWeCacheTheNewEntry()
    {
        _cache.Get((_worldID, _itemID)).Returns((CraftSummaryPoco)null);

        _ = await _craftRepository.GetBestCraft(_worldID, _itemID);

        _cache.Received(1).Get((_worldID, _itemID));
        _cache.Received(1).Add((_worldID, _itemID), Arg.Is<CraftSummaryPoco>(s => s.ItemID == _itemID && s.WorldID == _worldID));
    }


    [Test]
    public async Task GivenAGetBestCraft_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        var summary = new CraftSummaryPoco { WorldID = _worldID, ItemID = _itemID };
        _cache.Get((_worldID, _itemID)).Returns(summary);

        _ = await _craftRepository.GetBestCraft(_worldID, _itemID);

        _cache.Received(1).Get((_worldID, _itemID));
        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<CraftSummaryPoco>());
    }

    [Test]
    public async Task GivenAGetBestCraft_WhenTheIDIsValidButNoRecipesExist_ThenWeReturnNull()
    {
        _recipeRepository.Get(_recipeID).Returns((RecipePoco)null);

        var result = await _craftRepository.GetBestCraft(_worldID, _itemID);

        Assert.That(result, Is.Null);
        _cache.Received().Get((_worldID, _itemID));
        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<CraftSummaryPoco>());
    }

    [SetUp]
    public void SetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _calc.CalculateCraftingCostForItem(_worldID, _itemID).Returns((_recipeID, _craftingCost));

        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _priceRepository
            .Get(_worldID, _itemID)
            .Returns(new PricePoco { WorldID = _worldID, ItemID = _itemID });

        _recipeRepository = Substitute.For<IRecipeRepository>();
        _recipeRepository.Get(_recipeID).Returns(new RecipePoco { ID = _recipeID, TargetItemID = _itemID });
        _recipeRepository.GetAll().Returns(GetCrafts());

        _itemRepository = Substitute.For<IItemRepository>();
        _itemRepository.Get(_itemID).Returns(new ItemInfoPoco { ID = _itemID, Name = _itemName });

        _cache = Substitute.For<ICraftCache>();
        _cache
            .Get((_worldID, _itemID))
            .Returns(null, new CraftSummaryPoco() { ItemID = _itemID, Name = _itemName });

        _logger = Substitute.For<ILogger<CraftRepository>>();

        _craftRepository = new CraftRepository(
            _calc,
            _priceRepository,
            _recipeRepository,
            _itemRepository,
            _cache,
            _logger
        );
    }

    private List<RecipePoco> GetCrafts() => new()
    {
            new() { ID = _recipeID, TargetItemID = _itemID },
            new() { ID = _recipeID + 1, TargetItemID = _itemID + 1 }
        };
}
