using GilGoblin.Pocos;
using GilGoblin.Crafting;
using NSubstitute;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging.Abstractions;

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
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentItemID = -200;
        _recipes.GetRecipesForItem(inexistentItemID).Returns(Array.Empty<RecipePoco>());

        var result = _calc!.CalculateCraftingCostForItem(_worldID, inexistentItemID);

        _recipes.Received(1).GetRecipesForItem(inexistentItemID);
        _prices.DidNotReceiveWithAnyArgs().Get(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenARecipeExists_ThenWeTheReturnCraftingCost()
    {
        const int itemID = 1;
        const int ingredientID = 2;
        var market = GetNewMarketData;
        market.ItemID = itemID;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;

        var ingredient = new IngredientPoco(recipe.Ingredients.First())
        {
            Quantity = 10,
            ItemID = ingredientID
        };
        var ingredientMarket = GetNewMarketData;
        ingredientMarket.ItemID = ingredientID;
        recipe.Ingredients = new List<IngredientPoco>() { ingredient };
        var itemIDList = new List<int>() { itemID, ingredientID };
        itemIDList.Sort();
        MockReposForSingularTest(market, recipe, ingredientMarket);
        SetupMarketDataForIngredients(recipe);

        var result = _calc!.CalculateCraftingCostForItem(_worldID, itemID);

        _recipes.Received().GetRecipesForItem(itemID);
        _recipes.Received().GetRecipesForItem(ingredientID);
        _prices.Received().Get(_worldID, itemID);
        _prices.Received().Get(_worldID, ingredientID);
        Assert.That(result, Is.LessThan(int.MaxValue));
        Assert.That(result, Is.GreaterThan(ingredientMarket.AverageSoldNQ));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentRecipeID = -200;
        _recipes.Get(inexistentRecipeID).Returns(_ => null!);

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, inexistentRecipeID);

        _recipes.Received().Get(inexistentRecipeID);
        _prices.DidNotReceiveWithAnyArgs().Get(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists_WhenNoMarketDataFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;
        _recipes.Get(recipeID).Returns(recipe);
        _prices.Get(_worldID, default).ReturnsForAnyArgs(new MarketDataPoco());

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        _recipes.Received().Get(recipeID);
        _prices.ReceivedWithAnyArgs().Get(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists__ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;
        var marketData = GetNewMarketData;
        SetupBasicTestCase(recipe, marketData);
        SetupMarketDataForIngredients(recipe);

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        _recipes.Received().Get(recipeID);
        _recipes.Received().GetRecipesForItem(recipe.Ingredients[0].ItemID);
        _recipes.Received().GetRecipesForItem(recipe.Ingredients[1].ItemID);
        _recipes.DidNotReceive().GetRecipesForItem(recipe.TargetItemID);
        _prices.Received().Get(_worldID, Arg.Any<int>());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(1000));
    }

    private void SetupBasicTestCase(RecipePoco recipe, MarketDataPoco marketData)
    {
        var recipeID = recipe.RecipeID;
        _recipes.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
        _recipes.Get(recipeID).Returns(recipe);
        foreach (var ingredient in recipe.Ingredients)
            _recipes.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

        _prices.Get(marketData.WorldID, marketData.ItemID).Returns(marketData);
        _grocer.BreakdownRecipe(recipeID).Returns(recipe.Ingredients);
    }

    private void SetupMarketDataForIngredients(RecipePoco recipe, int worldID = 34)
    {
        foreach (var ingredient in recipe.Ingredients)
        {
            var ingredientMarketData = new MarketDataPoco()
            {
                ItemID = ingredient.ItemID,
                AverageListingPrice = 300,
                AverageSold = 280,
                WorldID = worldID,
                Name = "TestItem" + ingredient.ItemID,
            };
            _prices.Get(worldID, ingredient.ItemID).Returns(ingredientMarketData);
        }
    }

    private void MockReposForSingularTest(
        MarketDataPoco market,
        RecipePoco recipe,
        MarketDataPoco ingredientMarket
    )
    {
        _prices.Get(market.WorldID, market.ItemID).ReturnsForAnyArgs(market);
        _prices.Get(ingredientMarket.WorldID, ingredientMarket.ItemID).ReturnsForAnyArgs(market);
        _grocer.BreakdownRecipe(recipe.RecipeID).Returns(recipe.Ingredients);
        _recipes.GetRecipesForItem(market.ItemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(ingredientMarket.ItemID).Returns(Array.Empty<RecipePoco>());
        _recipes.Get(recipe.RecipeID).Returns(recipe);
    }

    private static MarketDataPoco GetNewMarketData =>
        new(1, _worldID, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);

    private static RecipePoco NewRecipe =>
        new(
            true,
            true,
            _targetItemID,
            _recipeID,
            254,
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
