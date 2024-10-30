using System;
using System.Data;
using System.Threading.Tasks;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
        _prices.DidNotReceive().Get(_worldId, inexistentItemId, Arg.Any<bool>());
        Assert.That(recipeId, Is.LessThan(0));
        Assert.That(cost, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task GivenCalculateCraftingCostForItem_WhenARecipeExists_ThenCraftingCostIsReturned()
    {
        var itemId = NewRecipe.TargetItemId;
        var recipe = NewRecipe;
        SetupForBasicItemCraftingCostCase(recipe, GetNewPrice());

        var (recipeId, cost) = await _calc.CalculateCraftingCostForItem(_worldId, itemId);

        _recipes.Received().Get(recipe.Id);
        await _grocer.Received(1).BreakdownRecipeById(recipe.Id);

        Assert.That(recipeId, Is.EqualTo(recipe.Id));
        _recipes.Received().GetRecipesForItem(itemId);
        _recipes.Received().GetRecipesForItem(NewRecipe.ItemIngredient0TargetId);
        // _prices.Received().Get(_worldId, itemId, false);
        // _prices.Received().Get(_worldId, ingredientId, false);
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnInexistentRecipe_ThenReturnError()
    {
        const int inexistentRecipeId = -200;
        _recipes.Get(Arg.Any<int>()).ReturnsNull();

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, inexistentRecipeId, false);

        _recipes.Received().Get(inexistentRecipeId);
        _prices.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        Assert.That(result, Is.EqualTo(_errorCost));
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipeAndNoMarketDataIsFound_ThenReturnErrorCost()
    {
        var recipe = NewRecipe;
        var recipeId = recipe.Id;
        _recipes.Get(recipeId).Returns(recipe);
        _prices.Get(_worldId, Arg.Any<int>(), Arg.Any<bool>()).ReturnsNull();

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId, false);

        Assert.That(result, Is.EqualTo(_errorCost));
        _recipes.Received().Get(recipeId);
        await _grocer.Received().BreakdownRecipeById(recipeId);
        _prices.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task WhenCalculatingCraftingCostForAnExistingRecipe_ThenReturnCraftingCost()
    {
        var recipe = NewRecipe;
        var recipeId = recipe.Id;
        var price = GetNewPrice();
        SetupBasicTestCase(recipe, price);
        SetupPricesForIngredients(recipe, GetAverageSalePricePoco(recipe.TargetItemId));

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId, false);

        _recipes.Received().Get(recipeId);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient0TargetId);
        _recipes.Received().GetRecipesForItem(recipe.ItemIngredient1TargetId);
        _recipes.DidNotReceive().GetRecipesForItem(recipe.TargetItemId);
        _prices.Received().Get(_worldId, Arg.Any<int>(), false);
    }

    [Test]
    public async Task WhenAnDataNotFoundExceptionOccurs_ThenAnErrorIsLoggedAndErrorCostReturned()
    {
        var recipeId = NewRecipe.Id;
        // _recipes.When(x => x.Get(recipeId)).Do(_ => throw new DataException());
        _recipes.Get(default).ThrowsForAnyArgs<DataException>();

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId, false);

        // Assert.That(result, Is.EqualTo(_errorCost));
        _logger.Received()
            .LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeId} in world {_worldId}"
            );
    }

    [Test]
    public async Task GivenCalculateCraftingCostForRecipe_WhenTheCostIsCached_ThenItIsReturnedImmediately()
    {
        var poco = new RecipeCostPoco(_recipeId, _worldId, false, 9001, DateTimeOffset.UtcNow.DateTime);
        _recipeCosts.GetAsync(_worldId, _recipeId).Returns(poco);

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, _recipeId, false);

        Assert.That(result, Is.EqualTo(poco.Amount));
        await _recipeCosts.Received(1).GetAsync(_worldId, _recipeId);
        _recipes.DidNotReceive().Get(Arg.Any<int>());
    }

    [Test]
    public async Task WhenAnUnexpectedExceptionOccurs_ThenAnErrorIsLoggedAndErrorCostReturned()
    {
        var recipeId = NewRecipe.Id;
        var errorMessage = "testMessageHere";
        _recipes.When(x => x.Get(recipeId)).Do(_ => throw new Exception(errorMessage));

        var result = await _calc.CalculateCraftingCostForRecipe(_worldId, recipeId, false);

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

        _prices.Get(price.WorldId, price.ItemId, Arg.Any<bool>()).Returns(price);
        _grocer.BreakdownRecipeById(recipeId).Returns(recipe.GetActiveIngredients());
    }

    private void SetupPricesForIngredients(RecipePoco recipe, AverageSalePricePoco averageSalePricePoco,
        int worldId = 34)
    {
        foreach (var ingredient in recipe.GetActiveIngredients())
        {
            var ingredientPrice = new PricePoco(ingredient.ItemId, worldId, false,
                averageSalePricePoco.Id) { AverageSalePrice = averageSalePricePoco };
            _prices.Get(worldId, ingredient.ItemId, false).Returns(ingredientPrice);
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        }
    }

    private void MockReposForSingularTest(
        PricePoco market,
        RecipePoco recipe,
        PricePoco ingredientMarket
    )
    {
        _recipeCosts.GetAsync(market.WorldId, recipe.Id).ReturnsNullForAnyArgs();
        _prices.Get(market.WorldId, market.ItemId, Arg.Any<bool>()).Returns(market);
        _prices.Get(ingredientMarket.WorldId, ingredientMarket.ItemId, Arg.Any<bool>()).Returns(market);
        var activeIngredients = recipe.GetActiveIngredients();
        _grocer.BreakdownRecipeById(recipe.Id).Returns(activeIngredients);
        foreach (var ingredient in activeIngredients)
        {
            _prices.Get(market.WorldId, ingredient.ItemId, Arg.Any<bool>()).Returns(market);
            _prices.Get(ingredientMarket.WorldId, ingredient.ItemId, Arg.Any<bool>()).Returns(ingredientMarket);
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        }

        _recipes.GetRecipesForItem(market.ItemId).Returns([recipe]);
        _recipes.GetRecipesForItem(ingredientMarket.ItemId).Returns([]);
        _recipes.Get(recipe.Id).Returns(recipe);
    }

    private void SetupForBasicItemCraftingCostCase(
        RecipePoco recipe,
        PricePoco ingredientMarketPrice
    )
    {
        var itemId = recipe.TargetItemId;
        var ingredientId = recipe.ItemIngredient0TargetId;
        recipe.TargetItemId = itemId;
        recipe.ResultQuantity = 1;
        recipe.ItemIngredient0TargetId = ingredientId;
        recipe.AmountIngredient0 = 10;
        var averageSalePricePoco = GetAverageSalePricePoco(itemId);
        var market = GetNewPrice() with
        {
            ItemId = itemId,
            IsHq = false,
            WorldId = ingredientMarketPrice.WorldId,
            AverageSalePrice = averageSalePricePoco
        };
        var ingredientPrice = ingredientMarketPrice with { ItemId = ingredientId };
        MockReposForSingularTest(market, recipe, ingredientPrice);
        SetupPricesForIngredients(recipe, averageSalePricePoco);
    }

    private static AverageSalePricePoco GetAverageSalePricePoco(int itemId)
    {
        var priceDataPoco = new PriceDataPoco("DC", 300, 280, 300);
        var priceDetail =
            new AverageSalePricePoco(itemId, false, 300, 280, 300) { DcDataPoint = priceDataPoco };
        return priceDetail;
    }

    private static PricePoco GetNewPrice() =>
        new(
            1,
            _worldId,
            false,
            300,
            200,
            400,
            600);

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