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
    public async Task GivenGetBestCrafts_WhenUnderConstruction_ThenAnEmptyResultIsReturned()
    {
        var result = await _craftRepository.GetBestCrafts(_worldID);

        Assert.That(result, Is.Empty);
    }

    // [Test]
    // public async Task GivenGetBestCrafts_WhenARecipeThrowsAnExeption_ThenAnErrorIsLoggedAndOthersRecipesAreReturned()
    // {
    //     var crafts = new List<RecipePoco>
    //     {
    //         new RecipePoco { ID = 1, TargetItemID = 111 },
    //         new RecipePoco { ID = 2, TargetItemID = 222 }
    //     };
    //     _recipeRepository.GetAll().Returns(crafts);
    //     _cache.Get((_worldID, crafts[1].ID)).Throws<ArithmeticException>();
    //     _calc.CalculateCraftingCostForItem(_worldID, crafts[0].TargetItemID).Returns((_worldID, 55));
    //     _calc.CalculateCraftingCostForItem(_worldID, crafts[1].TargetItemID).Returns((_worldID, 66));
    //     _recipeRepository.Get(crafts[0].ID).Returns(crafts[0]);
    //     _recipeRepository.Get(crafts[1].ID).Returns(crafts[1]);

    //     var result = await _craftRepository.GetBestCrafts(_worldID);

    //     // _logger
    //     //     .Received(1)
    //     //     .LogError($"Failed to calculate best craft for item {3} in world {_worldID}");
    //     Assert.That(result.Count, Is.EqualTo(1));
    // }

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
        _calc.CalculateCraftingCostForItem(_worldID, _itemID).Returns((_recipeID, _craftingCost));
        _recipeRepository.Get(_recipeID).Returns(new RecipePoco { ID = _recipeID });
        _priceRepository
            .Get(_worldID, _itemID)
            .Returns(new PricePoco { WorldID = _worldID, ItemID = _itemID });
        _itemRepository.Get(_itemID).Returns(new ItemInfoPoco { ID = _itemID, Name = _itemName });
        _cache
            .Get((_worldID, _itemID))
            .Returns(null, new CraftSummaryPoco() { ItemID = _itemID, Name = _itemName });
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _itemRepository = Substitute.For<IItemRepository>();
        _cache = Substitute.For<ICraftCache>();
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
}
