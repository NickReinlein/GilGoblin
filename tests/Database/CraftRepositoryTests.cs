using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class CraftRepositoryTests
{
    private CraftRepository _craftRepository;

    private ICraftingCalculator _calc;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IItemRepository _itemRepository;
    private ILogger<CraftRepository> _logger;

    public static readonly int WorldID = 22;
    public static readonly int ItemID = 6400;
    public static readonly int RecipeID = 444;
    public static readonly int CraftingCost = 777;
    public static readonly string ItemName = "Excalibur";

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenOtherRepositoriesAreCalled()
    {
        await _craftRepository.GetBestCraft(WorldID, ItemID);

        await _calc.Received().CalculateCraftingCostForItem(WorldID, ItemID);
        _recipeRepository.Received().Get(RecipeID);
        _priceRepository.Received().Get(WorldID, ItemID);
        _itemRepository.Received().Get(ItemID);
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsValid_ThenASummaryIsReturned()
    {
        var result = await _craftRepository.GetBestCraft(WorldID, ItemID);

        Assert.Multiple(() =>
        {
            Assert.That(result.CraftingCost, Is.EqualTo(CraftingCost));
            Assert.That(result.WorldID, Is.EqualTo(WorldID));
            Assert.That(result.ItemID, Is.EqualTo(ItemID));
            Assert.That(result.Recipe.ID, Is.EqualTo(RecipeID));
            Assert.That(result.Name, Is.EqualTo(ItemName));
        });
    }

    [Test]
    public async Task GivenGetBestCraft_WhenResultIsInvalid_ThenNullIsReturned()
    {
        _calc.CalculateCraftingCostForItem(WorldID, ItemID).Returns((0, 0));

        var result = await _craftRepository.GetBestCraft(WorldID, ItemID);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenGetBestCrafts_WhenUnderConstruction_ThenAnEmptyResultIsReturned()
    {
        var result = await _craftRepository.GetBestCrafts(WorldID);

        Assert.That(result, Is.Empty);
    }

    [SetUp]
    public void SetUp()
    {
        _calc.CalculateCraftingCostForItem(WorldID, ItemID).Returns((RecipeID, CraftingCost));
        _recipeRepository.Get(RecipeID).Returns(new RecipePoco { ID = RecipeID });
        _priceRepository
            .Get(WorldID, ItemID)
            .Returns(new PricePoco { WorldID = WorldID, ItemID = ItemID });
        _itemRepository.Get(ItemID).Returns(new ItemInfoPoco { ID = ItemID, Name = ItemName });
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _itemRepository = Substitute.For<IItemRepository>();
        _logger = Substitute.For<ILogger<CraftRepository>>();

        _craftRepository = new CraftRepository(
            _calc,
            _priceRepository,
            _recipeRepository,
            _itemRepository,
            _logger
        );
    }
}
