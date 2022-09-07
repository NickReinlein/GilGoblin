using GilGoblin.Pocos;
using GilGoblin.Web;
using GilGoblin.Crafting;
using NSubstitute;
using NUnit.Framework;
using Serilog;
using System.Linq.Expressions;

namespace GilGoblin.Test.Crafting;

[TestFixture]
public class CraftingCalculatorTest
{
    private readonly IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
    private readonly IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
    private readonly ILogger _log = Substitute.For<ILogger>();
    private CraftingCalculator? _calc;

    private static readonly int _errorCost = CraftingCalculator.ERROR_DEFAULT_COST;
    private static readonly int _worldID = 34; // Brynnhildr
    private static readonly int _firstItemID = 554;
    private static readonly int _secondItemID = 668;
    private static readonly int _secondRecipeID = 2000;
    private static readonly int _subItem1ID = 4100;
    private static readonly int _subItem2ID = 4200;
    private static readonly int _recipeID = 6044;
    private static readonly int _targetItemID = 955;

    [SetUp]
    public void SetUp()
    {
        _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway, _log);
    }

    [TearDown]
    public void TearDown()
    {
        _recipeGateway.ClearReceivedCalls();
        _marketDataGateway.ClearReceivedCalls();
        _log.ClearReceivedCalls();
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculateCraftingCostForItem_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentItemID = -200;
        _recipeGateway
            .GetRecipesForItem(inexistentItemID)
            .Returns(Array.Empty<RecipePoco>());

        var result = _calc!.CalculateCraftingCostForItem(_worldID, inexistentItemID);

        _recipeGateway.Received(1).GetRecipesForItem(inexistentItemID);
        _marketDataGateway.DidNotReceiveWithAnyArgs()
                          .GetMarketDataItems(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculateCraftingCostForItem_WhenARecipeExists_ThenReturnCraftingCost()
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
        _marketDataGateway
            .GetMarketDataItems(default, default!)
            .ReturnsForAnyArgs(new List<MarketDataPoco>() { market, ingredientMarket });

        _recipeGateway
            .GetRecipesForItem(itemID)
            .Returns(new List<RecipePoco>() { recipe });
        _recipeGateway
            .GetRecipesForItem(ingredientID)
            .Returns(Array.Empty<RecipePoco>());
        _recipeGateway
            .GetRecipe(recipe.RecipeID)
            .Returns(recipe);

        var result = _calc!.CalculateCraftingCostForItem(_worldID, itemID);

        _recipeGateway.Received().GetRecipesForItem(itemID);
        _recipeGateway.Received().GetRecipesForItem(ingredientID);
        _marketDataGateway.ReceivedWithAnyArgs().GetMarketDataItems(default, default!);
        Assert.That(result, Is.LessThan(int.MaxValue));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenNoRecipesExist_ThenReturnErrorCost()
    {
        var inexistentRecipeID = -200;
        _recipeGateway
            .GetRecipe(inexistentRecipeID)
            .Returns(_ => null!);

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, inexistentRecipeID);

        _recipeGateway.Received().GetRecipe(inexistentRecipeID);
        _marketDataGateway.DidNotReceiveWithAnyArgs().GetMarketDataItems(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }
    [Test]
    public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenARecipeExists_WhenNoMarketDataFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;
        _recipeGateway.GetRecipe(recipeID).Returns(recipe);
        _marketDataGateway
            .GetMarketDataItems(_worldID, default!)
            .ReturnsForAnyArgs(Array.Empty<MarketDataPoco>());

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        _recipeGateway.Received().GetRecipe(recipeID);
        _marketDataGateway.ReceivedWithAnyArgs().GetMarketDataItems(default, default!);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenARecipeExists__ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;
        _recipeGateway.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
        _recipeGateway.GetRecipe(recipeID).Returns(recipe);
        foreach (var ingredient in recipe.Ingredients)
            _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

        var marketData = GetNewMarketData;
        var ingredientMarketDataList = new List<MarketDataPoco>();
        foreach (var ingredient in recipe.Ingredients)
        {
            var tempData = GetNewMarketData;
            tempData.ItemID = ingredient.ItemID;
            ingredientMarketDataList.Add(tempData);
        }
        var returnMarketData = new List<MarketDataPoco>() { marketData };
        returnMarketData.AddRange(ingredientMarketDataList);
        _marketDataGateway.GetMarketDataItems(_worldID, Arg.Any<IEnumerable<int>>()).Returns(returnMarketData);

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        _recipeGateway
            .Received()
            .GetRecipe(recipeID);
        _recipeGateway
            .Received()
            .GetRecipesForItem(recipe.Ingredients[0].ItemID);
        _recipeGateway
            .Received()
            .GetRecipesForItem(recipe.Ingredients[1].ItemID);
        _recipeGateway
            .DidNotReceive()
            .GetRecipesForItem(recipe.TargetItemID);
        _marketDataGateway
            .Received()
            .GetMarketDataItems(_worldID, Arg.Any<IEnumerable<int>>());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(3000));
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownAnItem_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentItemID = 1033;
        _recipeGateway.GetRecipesForItem(inexistentItemID)
                      .Returns(Array.Empty<RecipePoco>());

        var result = _calc!.BreakdownItem(inexistentItemID);

        _recipeGateway.Received(1).GetRecipesForItem(inexistentItemID);
        _recipeGateway.DidNotReceiveWithAnyArgs().GetRecipe(default);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownAnItem_WhenItHas1Ingredient_ThenReturn1()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var recipeID = recipe.RecipeID;
        var subItem1 = new IngredientPoco(_subItem1ID, 10, recipeID);
        recipe.Ingredients = new List<IngredientPoco>() { subItem1 };
        var expectedTotalIngredientsCount = recipe.Ingredients
            .Select(x => x.Quantity)
            .Sum();
        _recipeGateway.GetRecipe(recipeID)
                      .Returns(recipe);
        _recipeGateway.GetRecipesForItem(itemID)
                      .Returns(new List<RecipePoco>() { recipe });
        _recipeGateway.GetRecipesForItem(_subItem1ID)
                      .Returns(Array.Empty<RecipePoco>());

        var result = _calc!.BreakdownItem(itemID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        _recipeGateway.Received(1).GetRecipesForItem(itemID);
        _recipeGateway.Received(1).GetRecipesForItem(_subItem1ID);
        _recipeGateway.Received(1).GetRecipe(recipeID);
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipe.Ingredients, Has.Count.EqualTo(1));
        });
    }

    // [Test]
    // public void GivenACraftingCalculator_WhenBreakingDownAnItem_WhenItHas2Ingredients_ThenReturn2()
    // {
    //     const int existentRecipeID = 1033;
    //     var recipePoco = NewRecipe;
    //     var expectedTotalIngredientsCount = recipePoco.Ingredients
    //         .Select(x => x.Quantity)
    //         .Sum();
    //     _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

    //     var result = _calc!.BreakdownRecipe(existentRecipeID);

    //     var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
    //     _recipeGateway.Received(1).GetRecipe(existentRecipeID);
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
    //         Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
    //         Assert.That(recipePoco.Ingredients, Has.Count.GreaterThanOrEqualTo(2));
    //     });
    // }

    // [Test]
    // public void GivenACraftingCalculator_WhenBreakingDownAnItem_WhenItHas3Ingredients_ThenReturn3()
    // {
    //     var recipe = NewRecipe;
    //     var recipeID = recipe.RecipeID;

    //     recipe.Ingredients.Add(new IngredientPoco(753, 5, recipeID));
    //     _recipeGateway.GetRecipe(recipeID).Returns(recipe);
    //     MockIngredientsToReturnEmpty(recipe.Ingredients);
    //     var expectedIngredientsSum = recipe.Ingredients.Select(x => x.Quantity).Sum();
    //     var expectedIngredientsCount = recipe.Ingredients.Count;

    //     var result = _calc!.BreakdownRecipe(recipeID);

    //     var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
    //     var resultIngredientsCount = result.Count();
    //     _recipeGateway.Received(1).GetRecipe(recipeID);
    //     _recipeGateway.ReceivedWithAnyArgs(3).GetRecipesForItem(Arg.Any<int>());
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
    //         Assert.That(resultIngredientsCount, Is.GreaterThanOrEqualTo(3));
    //         Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
    //     });
    // }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentRecipeID = 1033;
        var recipePoco = NewRecipe;
        recipePoco.Ingredients = new List<IngredientPoco>() { recipePoco.Ingredients.First() };
        _recipeGateway.GetRecipe(inexistentRecipeID)
                      .Returns(_ => null!);

        var result = _calc!.BreakdownRecipe(inexistentRecipeID);

        _recipeGateway.Received(1).GetRecipe(inexistentRecipeID);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas1Ingredient_ThenReturn1()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        recipePoco.Ingredients = new List<IngredientPoco>() { recipePoco.Ingredients.First() };
        var expectedTotalIngredientsCount = recipePoco.Ingredients
            .Select(x => x.Quantity)
            .Sum();
        _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

        var result = _calc!.BreakdownRecipe(existentRecipeID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        _recipeGateway.Received(1).GetRecipe(existentRecipeID);
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipePoco.Ingredients, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Ingredients_ThenReturn2()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        var expectedTotalIngredientsCount = recipePoco.Ingredients
            .Select(x => x.Quantity)
            .Sum();
        _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

        var result = _calc!.BreakdownRecipe(existentRecipeID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        _recipeGateway.Received(1).GetRecipe(existentRecipeID);
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipePoco.Ingredients, Has.Count.GreaterThanOrEqualTo(2));
        });
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas3Ingredients_ThenReturn3()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;

        recipe.Ingredients.Add(new IngredientPoco(753, 5, recipeID));
        _recipeGateway.GetRecipe(recipeID).Returns(recipe);
        MockIngredientsToReturnEmpty(recipe.Ingredients);
        var expectedIngredientsSum = recipe.Ingredients.Select(x => x.Quantity).Sum();
        var expectedIngredientsCount = recipe.Ingredients.Count;

        var result = _calc!.BreakdownRecipe(recipeID);

        var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
        var resultIngredientsCount = result.Count();
        _recipeGateway.Received(1).GetRecipe(recipeID);
        _recipeGateway.ReceivedWithAnyArgs(3).GetRecipesForItem(Arg.Any<int>());
        Assert.Multiple(() =>
        {
            Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
            Assert.That(resultIngredientsCount, Is.GreaterThanOrEqualTo(3));
            Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
        });
    }

    [Test]
    public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Levels_ThenReturnAllBrokenDown()
    {
        var firstRecipe = NewRecipe;
        var firstRecipeID = firstRecipe.RecipeID;
        Assume.That(firstRecipe.Ingredients.Count, Is.GreaterThanOrEqualTo(2));
        var subItem1 = GetNewMarketData;
        subItem1.ItemID = _subItem1ID;
        var subItem2 = GetNewMarketData;
        subItem2.ItemID = _subItem2ID;
        var secondRecipe = GetSecondRecipe(subItem1, subItem2);
        Assume.That(secondRecipe.Ingredients.Count, Is.GreaterThanOrEqualTo(2));
        var expectedIngredients = GetExpectedIngredientsFromRecipes(firstRecipe, secondRecipe);
        var expectedIngredientsSum = expectedIngredients.Select(x => x.Quantity).Sum();
        var expectedIngredientsCount = expectedIngredients.Count;
        Assume.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
        MockRecipeGatewayForTwoRecipes(firstRecipe, secondRecipe);

        var result = _calc!.BreakdownRecipe(firstRecipeID);

        //Ensure we check for every item and its respective potential breakdowns
        var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
        var resultIngredientsCount = result.Count();
        _recipeGateway.Received(1).GetRecipe(firstRecipeID);
        _recipeGateway.Received(1).GetRecipe(_secondRecipeID);
        _recipeGateway.Received(1).GetRecipesForItem(_firstItemID);
        _recipeGateway.Received(1).GetRecipesForItem(_secondItemID);
        _recipeGateway.Received(1).GetRecipesForItem(_subItem1ID);
        _recipeGateway.Received(1).GetRecipesForItem(_subItem2ID);
        Assert.Multiple(() =>
        {
            Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
            Assert.That(resultIngredientsCount, Is.EqualTo(expectedIngredientsCount));
        });
    }

    private void MockRecipeGatewayForTwoRecipes(RecipePoco firstRecipe, RecipePoco secondRecipe)
    {
        Assume.That(secondRecipe.Ingredients.Count, Is.GreaterThan(1));
        Assume.That(firstRecipe.Ingredients.Count, Is.GreaterThan(1));
        _recipeGateway.GetRecipe(firstRecipe.RecipeID).Returns(firstRecipe);
        _recipeGateway.GetRecipe(secondRecipe.RecipeID).Returns(secondRecipe);
        _recipeGateway.GetRecipesForItem(_firstItemID).Returns(Array.Empty<RecipePoco>());
        _recipeGateway.GetRecipesForItem(_secondItemID).Returns(new List<RecipePoco>() { secondRecipe });
        MockIngredientsToReturnEmpty(secondRecipe.Ingredients);
    }

    private void MockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
    {
        foreach (var ingredient in ingredients)
            _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(Array.Empty<RecipePoco>());
    }

    private static List<IngredientPoco> GetExpectedIngredientsFromRecipes(RecipePoco firstRecipe, RecipePoco secondRecipe)
    {
        var secondRecipeIngredients = secondRecipe.Ingredients;
        var expectedIngredients = new List<IngredientPoco>(secondRecipeIngredients);
        Assume.That(expectedIngredients, Is.Not.Empty);

        var firstRecipeIngredient = firstRecipe.Ingredients.FirstOrDefault()!;
        Assume.That(firstRecipeIngredient, Is.Not.Null);

        expectedIngredients.Add(new IngredientPoco(firstRecipeIngredient));
        Assume.That(expectedIngredients.Count, Is.GreaterThan(secondRecipeIngredients.Count));
        return expectedIngredients;
    }

    private static RecipePoco GetSecondRecipe(MarketDataPoco subItem1, MarketDataPoco subItem2)
    {
        var recipe = NewRecipe;
        recipe.RecipeID = _secondRecipeID;
        recipe.TargetItemID = _secondItemID;

        var ingredient1 = new IngredientPoco(subItem1.ItemID, 6, _secondRecipeID);
        var ingredient2 = new IngredientPoco(subItem2.ItemID, 7, _secondRecipeID);
        recipe.Ingredients = new List<IngredientPoco>() { ingredient1, ingredient2 };

        return recipe;
    }

    private static MarketDataPoco GetNewMarketData => new(1, _worldID, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);

    private static RecipePoco NewRecipe => new(true, true, _targetItemID, _recipeID, 254, 1, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, _firstItemID, _secondItemID, 0, 0, 0, 0, 0, 0, 0, 0);
}
