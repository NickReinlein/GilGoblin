using GilGoblin.Pocos;
using GilGoblin.Web;
using GilGoblin.Crafting;
using NSubstitute;
using NUnit.Framework;
using Serilog;
using System.Linq.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GilGoblin.Test.Crafting;

[TestFixture]
public class CraftingCalculatorTest
{
    private readonly IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
    private readonly IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
    private readonly IRecipeGrocer _grocer = Substitute.For<IRecipeGrocer>();
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
        _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway, _grocer, _log);
    }

    [TearDown]
    public void TearDown()
    {
        _recipeGateway.ClearReceivedCalls();
        _marketDataGateway.ClearReceivedCalls();
        _log.ClearReceivedCalls();
        _grocer.ClearReceivedCalls();
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenNoRecipesExist_ThenReturnErrorCost()
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
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForItem_WhenARecipeExists_ThenReturnCraftingCost()
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
        MockGatewaysForSingularTest(market, recipe, ingredientMarket);

        var result = _calc!.CalculateCraftingCostForItem(_worldID, itemID);

        _recipeGateway.Received().GetRecipesForItem(itemID);
        _recipeGateway.Received().GetRecipesForItem(ingredientID);
        _marketDataGateway.ReceivedWithAnyArgs().GetMarketDataItems(default, default!);
        Assert.That(result, Is.LessThan(int.MaxValue));
        Assert.That(result, Is.GreaterThan(ingredientMarket.AverageSoldNQ));
    }

    private void MockGatewaysForSingularTest(MarketDataPoco market, RecipePoco recipe, MarketDataPoco ingredientMarket)
    {
        _marketDataGateway.GetMarketDataItems(default, default!)
            .ReturnsForAnyArgs(new List<MarketDataPoco>() { market, ingredientMarket });
        _grocer.BreakdownRecipe(recipe.RecipeID).Returns(recipe.Ingredients);
        _recipeGateway.GetRecipesForItem(market.ItemID).Returns(new List<RecipePoco>() { recipe });
        _recipeGateway.GetRecipesForItem(ingredientMarket.ItemID).Returns(Array.Empty<RecipePoco>());
        _recipeGateway.GetRecipe(recipe.RecipeID).Returns(recipe);
    }

    [Test]
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenNoRecipesExist_ThenReturnErrorCost()
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
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists_WhenNoMarketDataFound_ThenReturnErrorCost()
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
    public void GivenACraftingCalculator_WhenCalculatingCraftingCostForRecipe_WhenARecipeExists__ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.RecipeID;
        var marketData = GetNewMarketData;
        var ingredientMarketDataList = SetupMarketDataForIngredients(recipe);
        SetupBasicTestCase(recipe, marketData, ingredientMarketDataList);

        var result = _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);


        _recipeGateway.Received().GetRecipe(recipeID);
        _recipeGateway.Received().GetRecipesForItem(recipe.Ingredients[0].ItemID);
        _recipeGateway.Received().GetRecipesForItem(recipe.Ingredients[1].ItemID);
        _recipeGateway.DidNotReceive().GetRecipesForItem(recipe.TargetItemID);
        _marketDataGateway.Received().GetMarketDataItems(_worldID, Arg.Any<IEnumerable<int>>());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(3000));
    }

    private void SetupBasicTestCase(RecipePoco recipe, MarketDataPoco marketData, List<MarketDataPoco> ingredientMarketDataList)
    {
        var recipeID = recipe.RecipeID;
        _recipeGateway.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
        _recipeGateway.GetRecipe(recipeID).Returns(recipe);
        foreach (var ingredient in recipe.Ingredients)
            _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

        var returnMarketData = new List<MarketDataPoco>() { marketData };
        returnMarketData.AddRange(ingredientMarketDataList);
        _marketDataGateway.GetMarketDataItems(_worldID, Arg.Any<IEnumerable<int>>())
            .Returns(returnMarketData);
        _grocer.BreakdownRecipe(recipeID).Returns(recipe.Ingredients);
    }

    private static List<MarketDataPoco> SetupMarketDataForIngredients(RecipePoco recipe)
    {
        var ingredientMarketDataList = new List<MarketDataPoco>();
        foreach (var ingredient in recipe.Ingredients)
        {
            var tempData = GetNewMarketData;
            tempData.ItemID = ingredient.ItemID;
            ingredientMarketDataList.Add(tempData);
        }

        return ingredientMarketDataList;
    }

    private static MarketDataPoco GetNewMarketData => new(1, _worldID, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);

    private static RecipePoco NewRecipe => new(true, true, _targetItemID, _recipeID, 254, 1, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, _firstItemID, _secondItemID, 0, 0, 0, 0, 0, 0, 0, 0);
}
