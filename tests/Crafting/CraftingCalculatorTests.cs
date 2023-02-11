using GilGoblin.Crafting;
using GilGoblin.Extension;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Crafting;

public class CraftingCalculatorTests
{
    private readonly IRecipeRepository _recipes = Substitute.For<IRecipeRepository>();
    private readonly IPriceRepository _prices = Substitute.For<IPriceRepository>();
    private readonly IRecipeGrocer _grocer = Substitute.For<IRecipeGrocer>();
    private readonly ILogger<CraftingCalculator> _log =
        NullLoggerFactory.Instance.CreateLogger<CraftingCalculator>();
    private CraftingCalculator? _calc;

    private static readonly int _errorCost = CraftingCalculator.ERROR_DEFAULT_COST;
    private static readonly int _worldID = 34; // Brynnhildr
    private static readonly int _firstItemID = 554;
    private static readonly int _secondItemID = 668;
    private static readonly int _recipeID = 6044;
    private static readonly int _targetItemID = 955;

    [SetUp]
    public void SetUp()
    {
        _calc = new CraftingCalculator(_recipes, _prices, _grocer, _log);
    }

    [TearDown]
    public void TearDown()
    {
        _recipes.ClearReceivedCalls();
        _prices.ClearReceivedCalls();
        _grocer.ClearReceivedCalls();
    }

    [Test]
    public async Task GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentItemID = -200;
        _recipes.GetRecipesForItem(inexistentItemID).Returns(Array.Empty<RecipePoco>());

        var result = await _calc!.CalculateCraftingCostForItem(_worldID, inexistentItemID);

        await _recipes.Received(1).GetRecipesForItem(inexistentItemID);
        await _prices.DidNotReceiveWithAnyArgs().Get(_worldID, inexistentItemID, _prices.DidNotReceiveWithAnyArgs().GetWorldString());
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenARecipeExists_ThenWeTheReturnCraftingCost()
    {
        const int itemID = 1;
        const int ingredientID = 2;
        var market = GetNewPrice;
        market.ItemID = itemID;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var ingredientMarket = GetNewPrice;
        ingredientMarket.ItemID = ingredientID;
        recipe.ItemIngredient0TargetID = ingredientID;
        recipe.AmountIngredient0 = 10;

        var itemIDList = new List<int>() { itemID, ingredientID };
        itemIDList.Sort();
        MockReposForSingularTest(market, recipe, ingredientMarket);
        SetupPricesForIngredients(recipe);

        var result = await _calc!.CalculateCraftingCostForItem(_worldID, itemID);

        await _recipes.Received().GetRecipesForItem(itemID);
        await _recipes.Received().GetRecipesForItem(ingredientID);
        await _prices.Received().Get(_worldID, itemID, _prices.Received().GetWorldString());
        await _prices.Received().Get(_worldID, ingredientID, _prices.Received().GetWorldString());
        Assert.That(result, Is.LessThan(int.MaxValue));
        Assert.That(result, Is.GreaterThan(ingredientMarket.AverageSoldNQ));
    }

    [Test]
    public async Task GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentRecipeID = -200;
        _recipes.Get(Arg.Any<int>()).ReturnsNull();

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, inexistentRecipeID);

        await _recipes.Received().Get(inexistentRecipeID);
        await _prices.DidNotReceiveWithAnyArgs().Get(_worldID, inexistentRecipeID, _prices.DidNotReceiveWithAnyArgs().GetWorldString());
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists_WhenNoMarketDataFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.ID;
        _recipes.Get(recipeID).Returns(recipe);
        _prices.Get(_worldID, Arg.Any<int>(), _prices.GetWorldString()).ReturnsNull();

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        await _recipes.Received().Get(recipeID);
        await _prices.ReceivedWithAnyArgs().Get(default, default!, _prices.ReceivedWithAnyArgs().GetWorldString());
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists__ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.ID;
        var price = GetNewPrice;
        SetupBasicTestCase(recipe, price);
        SetupPricesForIngredients(recipe);

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        await _recipes.Received().Get(recipeID);
        await _recipes.Received().GetRecipesForItem(recipe.ItemIngredient0TargetID);
        await _recipes.Received().GetRecipesForItem(recipe.ItemIngredient1TargetID);
        await _recipes.DidNotReceive().GetRecipesForItem(recipe.TargetItemID);
        await _prices.Received().Get(_worldID, Arg.Any<int>(), _prices.Received().GetWorldString());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(1000));
    }

    private void SetupBasicTestCase(RecipePoco recipe, PricePoco price)
    {
        var recipeID = recipe.ID;
        _recipes.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
        _recipes.Get(recipeID).Returns(recipe);
        foreach (var ingredient in recipe.GetActiveIngredients())
            _recipes.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

        _prices.Get(price.WorldID, price.ItemID,
        _prices.GetWorldString()).Returns(price);
        _grocer.BreakdownRecipeById(recipeID).Returns(recipe.GetActiveIngredients());
    }

    private void SetupPricesForIngredients(RecipePoco recipe, int worldID = 34)
    {
        foreach (var ingredient in recipe.GetActiveIngredients())
        {
            var ingredientPrice = new PricePoco()
            {
                ItemID = ingredient.ItemID,
                AverageListingPrice = 300,
                AverageSold = 280,
                WorldID = worldID
            };
            _prices.Get(worldID, ingredient.ItemID, _prices.GetWorldString()).Returns(ingredientPrice);
        }
    }

    private void MockReposForSingularTest(
        PricePoco market,
        RecipePoco recipe,
        PricePoco ingredientMarket
    )
    {
        _prices.Get(market.WorldID, market.ItemID, _prices.GetWorldString()).ReturnsForAnyArgs(market);
        _prices.Get(ingredientMarket.WorldID, ingredientMarket.ItemID, _prices.GetWorldString()).ReturnsForAnyArgs(market);
        _grocer.BreakdownRecipeById(recipe.ID).Returns(recipe.GetActiveIngredients());
        _recipes.GetRecipesForItem(market.ItemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(ingredientMarket.ItemID).Returns(Array.Empty<RecipePoco>());
        _recipes.Get(recipe.ID).Returns(recipe);
    }

    private static PricePoco GetNewPrice => new(1, _worldID, 1, 300, 200, 400, 600, 400, 800);

    private static RecipePoco NewRecipe =>
        new(
            true,
            true,
            _targetItemID,
            _recipeID,
            1,
            3,
            4,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            _firstItemID,
            _secondItemID,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        );
}
