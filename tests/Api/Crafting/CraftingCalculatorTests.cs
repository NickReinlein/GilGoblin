using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Crafting;

public class CraftingCalculatorTests
{
    private CraftingCalculator _calc;

    private IRecipeRepository _recipes;
    private IPriceRepository<PricePoco> _prices;
    private IRecipeGrocer _grocer;
    private IRecipeCostRepository _recipeCosts;
    private ILogger<CraftingCalculator> _logger;

    private static readonly int _errorCost = CraftingCalculator.ErrorDefaultCost;
    private static readonly int _worldId = 34; // Brynnhildr
    private static readonly int _firstItemId = 554;
    private static readonly int _secondItemId = 668;
    private static readonly int _recipeId = 6044;
    private static readonly int _targetItemId = 955;

    [SetUp]
    public void SetUp()
    {
        _recipes = Substitute.For<IRecipeRepository>();
        _grocer = Substitute.For<IRecipeGrocer>();
        _prices = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeCosts = Substitute.For<IRecipeCostRepository>();
        _logger = Substitute.For<ILogger<CraftingCalculator>>();

        _calc = new CraftingCalculator(_recipes, _prices, _recipeCosts, _grocer, _logger);
    }

    [Test]
    public async Task GivenCalculateCraftingCostForItem_WhenNoRecipeExists_ThenErrorCostIsReturned()
    {
        var inexistentItemId = -200;
        _recipes.GetRecipesForItem(inexistentItemId).Returns([]);

        var (recipeId, cost) = await _calc.CalculateCraftingCostForItem(
            _worldId,
            inexistentItemId
        );

        _recipes.DidNotReceive().GetRecipesForItem(inexistentItemId);
        _prices.DidNotReceive().Get(_worldId, inexistentItemId);
        Assert.That(recipeId, Is.LessThan(0));
        Assert.That(cost, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task GivenCalculateCraftingCostForItem_WhenARecipeExists_ThenCraftingCostIsReturned()
    {
        var itemId = NewRecipe.TargetItemId;
        var ingredientId = NewRecipe.ItemIngredient0TargetId;
        var recipe = NewRecipe;
        var ingredientMarketPrice = GetNewPrice;
        SetupForBasicItemCraftingCostCase(recipe, ingredientMarketPrice);

        var (recipeId, cost) = await _calc.CalculateCraftingCostForItem(_worldId, itemId);

        _recipes.Received().GetRecipesForItem(itemId);
        _recipes.Received().GetRecipesForItem(ingredientId);
        _prices.Received().Get(_worldId, itemId);
        _prices.Received().Get(_worldId, ingredientId);
        Assert.Multiple(() =>
        {
            Assert.That(recipeId, Is.EqualTo(recipe.Id));
            Assert.That(cost, Is.LessThan(int.MaxValue));
            Assert.That(cost, Is.GreaterThan(ingredientMarketPrice.AverageSoldNQ));
        });
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnInexistentRecipe_ThenReturnErrorCost()
    {
        const int inexistentRecipeId = -200;
        _recipes.Get(Arg.Any<int>()).ReturnsNull();

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, inexistentRecipeId);

        _recipes.Received().Get(inexistentRecipeId);
        _prices.DidNotReceiveWithAnyArgs().Get(_worldId, inexistentRecipeId);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipeAndNoMarketDataIsFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeId = recipe.Id;
        _recipes.Get(recipeId).Returns(recipe);
        _prices.Get(_worldId, Arg.Any<int>()).ReturnsNull();

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId);

        Assert.That(result, Is.EqualTo(_errorCost));
        _recipes.Received().Get(recipeId);
        await _grocer.Received().BreakdownRecipeById(recipeId);
        _prices.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipe_ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeId = recipe.Id;
        var price = GetNewPrice;
        SetupBasicTestCase(recipe, price);
        SetupPricesForIngredients(recipe);

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId);

        _recipes.Received().Get(recipeId);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient0TargetId);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient1TargetId);
        _recipes.DidNotReceive().GetRecipesForItem(recipe.TargetItemId);
        _prices.Received().Get(_worldId, Arg.Any<int>());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(1000));
    }

    [Test]
    public void GivenAddPricesToIngredients_WithValidData_ThenTheResultIsValid()
    {
        var testIngredient = NewRecipe.GetIngredientsList().First();
        var ingredients = new List<IngredientPoco> { testIngredient };
        var price = new PricePoco { ItemId = testIngredient.ItemId, WorldId = _worldId };
        var prices = new List<PricePoco> { price };

        var result = _calc.AddPricesToIngredients(ingredients, prices);

        var craftIngredient = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(craftIngredient.ItemId, Is.EqualTo(testIngredient.ItemId));
            Assert.That(craftIngredient.Quantity, Is.EqualTo(testIngredient.Quantity));
            Assert.That(craftIngredient.Price, Is.EqualTo(price));
        });
    }

    [Test]
    public void GivenAddPricesToIngredients_WhenThereAreDuplicateIngredients_ThenTheResultCombinesDuplicates()
    {
        var testIngredient = NewRecipe.GetIngredientsList().First();
        var testIngredient2 = testIngredient;
        testIngredient2.Quantity *= 3;
        testIngredient2.RecipeId = 9911;
        var ingredients = new List<IngredientPoco> { testIngredient, testIngredient2 };
        var price = new PricePoco { ItemId = testIngredient.ItemId, WorldId = _worldId };
        var prices = new List<PricePoco> { price };

        var result = _calc.AddPricesToIngredients(ingredients, prices);

        var ingredientCount = testIngredient.Quantity + testIngredient2.Quantity;
        var craftIngredient = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(craftIngredient.ItemId, Is.EqualTo(testIngredient.ItemId));
            Assert.That(craftIngredient.Quantity, Is.EqualTo(ingredientCount));
            Assert.That(craftIngredient.Price, Is.EqualTo(price));
        });
    }

    [Test]
    public void GivenAddPricesToIngredients_WithInvalidData_ThenNullIsReturned()
    {
        var ingredients = new List<IngredientPoco> { NewRecipe.GetIngredientsList().First() };
        var prices = new List<PricePoco> { new() { WorldId = _worldId, ItemId = 222 } };

        var result = _calc.AddPricesToIngredients(ingredients, prices);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task WhenAnDataNotFoundExceptionOccurs_ThenAnErrorIsLoggedAndErrorCostReturned()
    {
        var recipeId = NewRecipe.Id;
        _recipes.When(x => x.Get(recipeId)).Do(_ => throw new DataException());

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId);

        Assert.That(result, Is.EqualTo(_errorCost));
        _logger
            .Received()
            .LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeId} in world {_worldId}"
            );
    }


    [Test]
    public void GivenGetIngredientPrice_WhenThereIsNoResult_ThenAnExceptionIsThrown()
    {
        _prices.Get(Arg.Any<int>(), Arg.Any<int>()).Returns((PricePoco)null!);

        Assert.Throws<DataException>(() =>
            _calc.GetIngredientPrices(_worldId, _firstItemId, NewRecipe.GetIngredientsList()));
    }

    [Test]
    public async Task GivenCalculateCraftingCostForRecipe_WhenTheCostIsCached_ThenItIsReturnedImmediately()
    {
        var poco = new RecipeCostPoco { RecipeId = _recipeId, WorldId = _worldId, Cost = 9001 };
        _recipeCosts.GetAsync(_worldId, _recipeId).Returns(poco);

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, _recipeId);

        Assert.That(result, Is.EqualTo(poco.Cost));
        await _recipeCosts.Received(1).GetAsync(_worldId, _recipeId);
        _recipes.DidNotReceive().Get(Arg.Any<int>());
    }

    [Test]
    public async Task GivenGetLowestCraftingCost_WhenTheCostIsCached_ThenItIsReturnedImmediately()
    {
        var poco = new RecipeCostPoco { RecipeId = _recipeId, WorldId = _worldId, Cost = 9001 };
        _recipeCosts.GetAsync(_worldId, _recipeId).Returns(poco);

        var result = await _calc.GetLowestCraftingCost(_worldId, new List<RecipePoco>() { NewRecipe });

        Assert.That(result.Item1, Is.EqualTo(poco.RecipeId));
        Assert.That(result.Item2, Is.EqualTo(poco.Cost));
        await _recipeCosts.Received(1).GetAsync(_worldId, _recipeId);
        _recipes.DidNotReceive().Get(Arg.Any<int>());
    }

    [Test]
    public async Task WhenAnUnexpectedExceptionOccurs_ThenAnErrorIsLoggedAndErrorCostReturned()
    {
        var recipeId = NewRecipe.Id;
        var errorMessage = "testMessageHere";
        _recipes.When(x => x.Get(recipeId)).Do(_ => throw new Exception(errorMessage));

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId);

        Assert.That(result, Is.EqualTo(_errorCost));
        _logger.Received().LogError($"Failed to calculate crafting cost: {errorMessage}");
    }

    private void SetupBasicTestCase(RecipePoco recipe, PricePoco price)
    {
        var recipeId = recipe.Id;
        _recipes.GetRecipesForItem(recipeId).Returns([recipe]);
        _recipes.Get(recipeId).Returns(recipe);
        foreach (var ingredient in recipe.GetActiveIngredients())
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);

        _prices.Get(price.WorldId, price.ItemId).Returns(price);
        _grocer.BreakdownRecipeById(recipeId).Returns(recipe.GetActiveIngredients());
    }

    private void SetupPricesForIngredients(RecipePoco recipe, int worldId = 34)
    {
        foreach (var ingredient in recipe.GetActiveIngredients())
        {
            var ingredientPrice = new PricePoco()
            {
                ItemId = ingredient.ItemId, AverageListingPrice = 300, AverageSold = 280, WorldId = worldId
            };
            _prices.Get(worldId, ingredient.ItemId).Returns(ingredientPrice);
        }
    }

    private void MockReposForSingularTest(
        PricePoco market,
        RecipePoco recipe,
        BasePricePoco ingredientMarket
    )
    {
        _prices.Get(market.WorldId, market.ItemId).ReturnsForAnyArgs(market);
        _prices.Get(ingredientMarket.WorldId, ingredientMarket.ItemId).ReturnsForAnyArgs(market);
        _grocer.BreakdownRecipeById(recipe.Id).Returns(recipe.GetActiveIngredients());
        _recipes.GetRecipesForItem(market.ItemId).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(ingredientMarket.ItemId).Returns([]);
        _recipes.Get(recipe.Id).Returns(recipe);
    }

    private void SetupForBasicItemCraftingCostCase(
        RecipePoco recipe,
        BasePricePoco ingredientMarketPrice
    )
    {
        var itemId = recipe.TargetItemId;
        var ingredientId = recipe.ItemIngredient0TargetId;
        var market = GetNewPrice;
        market.ItemId = itemId;
        recipe.TargetItemId = itemId;
        recipe.ResultQuantity = 1;
        ingredientMarketPrice.ItemId = ingredientId;
        recipe.ItemIngredient0TargetId = ingredientId;
        recipe.AmountIngredient0 = 10;
        MockReposForSingularTest(market, recipe, ingredientMarketPrice);
        SetupPricesForIngredients(recipe);
    }

    private static PricePoco GetNewPrice => new(1, _worldId, 1, 300, 200, 400, 600, 400, 800);

    private static RecipePoco NewRecipe =>
        new(
            true,
            true,
            _targetItemId,
            _recipeId,
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
            _firstItemId,
            _secondItemId,
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