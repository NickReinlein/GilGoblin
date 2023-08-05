using GilGoblin.Crafting;
using GilGoblin.Exceptions;
using GilGoblin.Extensions;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Crafting;

public class CraftingCalculatorTests
{
    private IRecipeRepository _recipes;

    private IPriceRepository<PricePoco> _prices;
    private IRecipeGrocer _grocer;
    private ILogger<CraftingCalculator> _logger;
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
        _recipes = Substitute.For<IRecipeRepository>();
        _grocer = Substitute.For<IRecipeGrocer>();
        _prices = Substitute.For<IPriceRepository<PricePoco>>();
        _logger = Substitute.For<ILogger<CraftingCalculator>>();
        _calc = new CraftingCalculator(_recipes, _prices, _grocer, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _recipes.ClearReceivedCalls();
        _prices.ClearReceivedCalls();
        _grocer.ClearReceivedCalls();
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForItemWhenNoRecipeExists_ThenReturnErrorCost()
    {
        var inexistentItemID = -200;
        _recipes.GetRecipesForItem(inexistentItemID).Returns(Array.Empty<RecipePoco>());

        var result = await _calc!.CalculateCraftingCostForItem(_worldID, inexistentItemID);

        _recipes.Received(1).GetRecipesForItem(inexistentItemID);
        _prices.DidNotReceiveWithAnyArgs().Get(_worldID, inexistentItemID);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForItemWhenARecipeExists_ThenWeTheReturnCraftingCost()
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

        _recipes.Received().GetRecipesForItem(itemID);
        _recipes.Received().GetRecipesForItem(ingredientID);
        _prices.Received().Get(_worldID, itemID);
        _prices.Received().Get(_worldID, ingredientID);
        Assert.That(result, Is.LessThan(int.MaxValue));
        Assert.That(result, Is.GreaterThan(ingredientMarket.AverageSoldNQ));
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnInexistentRecipe_ThenReturnErrorCost()
    {
        var inexistentRecipeID = -200;
        _recipes.Get(Arg.Any<int>()).ReturnsNull();

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, inexistentRecipeID);

        _recipes.Received().Get(inexistentRecipeID);
        _prices.DidNotReceiveWithAnyArgs().Get(_worldID, inexistentRecipeID);
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipeAndNoMarketDataIsFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.ID;
        _recipes.Get(recipeID).Returns(recipe);
        _prices.Get(_worldID, Arg.Any<int>()).ReturnsNull();

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        Assert.That(result, Is.EqualTo(_errorCost));
        _recipes.Received().Get(recipeID);
        await _grocer.Received().BreakdownRecipeById(recipeID);
        _prices.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipe_ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.ID;
        var price = GetNewPrice;
        SetupBasicTestCase(recipe, price);
        SetupPricesForIngredients(recipe);

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        _recipes.Received().Get(recipeID);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient0TargetID);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient1TargetID);
        _recipes.DidNotReceive().GetRecipesForItem(recipe.TargetItemID);
        _prices.Received().Get(_worldID, Arg.Any<int>());
        Assert.That(result, Is.LessThan(100000000));
        Assert.That(result, Is.GreaterThan(1000));
    }

    [Test]
    public void WhenAddingPricesToTheIngredientListWithPriceMatches_ThenTheResultContainsThePrice()
    {
        var testIngredient = NewRecipe.GetIngredientsList().First();
        var ingredients = new List<IngredientPoco> { testIngredient };
        var price = new PricePoco { ItemID = testIngredient.ItemID, WorldID = _worldID };
        var prices = new List<PricePoco> { price };

        var result = CraftingCalculator.AddPricesToIngredients(ingredients, prices);

        var craftIngredient = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(craftIngredient.ItemID, Is.EqualTo(testIngredient.ItemID));
            Assert.That(craftIngredient.Quantity, Is.EqualTo(testIngredient.Quantity));
            Assert.That(craftIngredient.Price, Is.EqualTo(price));
        });
    }

    [Test]
    public void WhenAddingPricesToTheIngredientListWithoutPriceMatches_ThenADataNotFoundExceptionIsThrown()
    {
        var ingredients = new List<IngredientPoco> { NewRecipe.GetIngredientsList().First() };
        var prices = new List<PricePoco>
        {
            new PricePoco { ItemID = 222, WorldID = _worldID }
        };

        Assert.Throws<InvalidOperationException>(
            () => CraftingCalculator.AddPricesToIngredients(ingredients, prices)
        );
    }

    [Test]
    public async Task WhenAnDataNotFoundExceptionOccurs_ThenAnErorIsLoggedAndErrorCostReturned()
    {
        var recipeID = NewRecipe.ID;
        _recipes.When(x => x.Get(recipeID)).Do(_ => throw new DataNotFoundException());

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        Assert.That(result, Is.EqualTo(_errorCost));
        _logger
            .Received()
            .LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeID} in world {_worldID}"
            );
    }

    [Test]
    public async Task WhenAnUnexpectedExceptionOccurs_ThenAnErorIsLoggedAndErrorCostReturned()
    {
        var recipeID = NewRecipe.ID;
        var errorMessage = "testMessageHere";
        _recipes.When(x => x.Get(recipeID)).Do(_ => throw new Exception(errorMessage));

        var result = await _calc!.CalculateCraftingCostForRecipe(_worldID, recipeID);

        Assert.That(result, Is.EqualTo(_errorCost));
        _logger.Received().LogError($"Failed to calculate crafting cost: {errorMessage}");
    }

    private void SetupBasicTestCase(RecipePoco recipe, PricePoco price)
    {
        var recipeID = recipe.ID;
        _recipes.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
        _recipes.Get(recipeID).Returns(recipe);
        foreach (var ingredient in recipe.GetActiveIngredients())
            _recipes.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

        _prices.Get(price.WorldID, price.ItemID).Returns(price);
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
            _prices.Get(worldID, ingredient.ItemID).Returns(ingredientPrice);
        }
    }

    private void MockReposForSingularTest(
        PricePoco market,
        RecipePoco recipe,
        PricePoco ingredientMarket
    )
    {
        _prices.Get(market.WorldID, market.ItemID).ReturnsForAnyArgs(market);
        _prices.Get(ingredientMarket.WorldID, ingredientMarket.ItemID).ReturnsForAnyArgs(market);
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
