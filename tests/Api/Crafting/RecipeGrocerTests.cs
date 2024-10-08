using System.Collections.Generic;
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

public class RecipeGrocerTests
{
    private RecipeGrocer _grocer;
    private IRecipeRepository _recipes;
    private ILogger<RecipeGrocer> _logger;

    private static readonly int _firstRecipeId = 6044;
    private static readonly int _secondRecipeId = 2000;
    private static readonly int _firstItemId = 554;
    private static readonly int _secondItemId = 668;
    private static readonly int _subItem0Id = 4000;
    private static readonly int _subItem1Id = 4100;
    private static readonly int _subItem2Id = 4200;
    private static readonly int _targetItemId = 955;

    [SetUp]
    public void SetUp()
    {
        _recipes = Substitute.For<IRecipeRepository>();
        Substitute.For<IPriceRepository<PricePoco>>();
        _logger = Substitute.For<ILogger<RecipeGrocer>>();
        _grocer = new RecipeGrocer(_recipes, _logger);
    }

    [Test]
    public async Task GivenABreakdownItem_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentItemId = 1033;
        _recipes.GetRecipesForItem(inexistentItemId).Returns([]);

        var result = await _grocer.BreakdownItem(inexistentItemId);

        _recipes.Received(1).GetRecipesForItem(inexistentItemId);
        _recipes.DidNotReceiveWithAnyArgs().Get(inexistentItemId);
        Assert.That(result.GetIngredientsCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas1Ingredient_ThenReturn1()
    {
        const int itemId = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemId = itemId;
        recipe.ResultQuantity = 1;
        var subItem1 = new IngredientPoco(_subItem0Id, 10, recipe.Id);
        recipe.AmountIngredient0 = subItem1.Quantity;
        recipe.ItemIngredient0TargetId = subItem1.ItemId;
        var activeIngredients = recipe.GetActiveIngredients();
        foreach (var ingredient in activeIngredients)
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        var expectedTotalIngredients = activeIngredients.GetIngredientsSum();
        _recipes.Get(recipe.Id).Returns(recipe);
        _recipes.GetRecipesForItem(itemId).Returns([recipe]);
        _recipes.GetRecipesForItem(_subItem0Id).Returns([]);

        var result = await _grocer.BreakdownItem(itemId);

        _recipes.Received(1).GetRecipesForItem(itemId);
        _recipes.Received(1).GetRecipesForItem(_subItem0Id);
        _recipes.Received(1).Get(recipe.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas2Ingredients_ThenReturn2()
    {
        const int itemId = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemId = itemId;
        recipe.ResultQuantity = 1;
        var subItem0 = new IngredientPoco(_subItem0Id, 10, recipe.Id);
        var subItem1 = new IngredientPoco(_subItem1Id, 4, recipe.Id);
        recipe.ItemIngredient0TargetId = subItem0.ItemId;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetId = subItem1.ItemId;
        recipe.AmountIngredient1 = subItem1.Quantity;
        var expectedTotalIngredients = recipe.GetActiveIngredients().GetIngredientsSum();
        _recipes.Get(recipe.Id).Returns(recipe);
        _recipes.GetRecipesForItem(itemId).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem0Id).Returns([]);
        _recipes.GetRecipesForItem(_subItem1Id).Returns([]);

        var result = await _grocer.BreakdownItem(itemId);

        var resultTotalIngredients = result.GetIngredientsSum();
        _recipes.Received(1).GetRecipesForItem(itemId);
        _recipes.Received(1).GetRecipesForItem(_subItem0Id);
        _recipes.Received(1).GetRecipesForItem(_subItem1Id);
        _recipes.Received(1).Get(recipe.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas3Ingredients_ThenReturn3()
    {
        const int itemId = 2344;
        var recipe = SetupBreakingDown3ForItem(itemId);

        var result = await _grocer.BreakdownItem(itemId);

        _recipes.Received(1).GetRecipesForItem(itemId);
        _recipes.Received(1).GetRecipesForItem(_subItem0Id);
        _recipes.Received(1).GetRecipesForItem(_subItem1Id);
        _recipes.Received(1).GetRecipesForItem(_secondItemId);
        _recipes.Received(1).Get(recipe.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(3));
            Assert.That(
                result.GetIngredientsSum(),
                Is.EqualTo(recipe.GetActiveIngredients().GetIngredientsSum())
            );
        });
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas2Levels_ThenReturnAllIngredients()
    {
        var expectedIngredientsCount = Setup2LevelItemTest();

        var result = await _grocer.BreakdownItem(_firstItemId);

        _recipes.Received(1).GetRecipesForItem(_firstItemId);
        _recipes.Received(1).Get(_firstRecipeId);
        _recipes.Received(1).GetRecipesForItem(_subItem0Id);
        _recipes.Received(1).GetRecipesForItem(_subItem1Id);
        _recipes.Received(1).Get(_secondRecipeId);
        _recipes.Received(1).GetRecipesForItem(_subItem2Id);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedIngredientsCount));
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownItem_When2RecipesExist_ThenIngredientsForBothAreReturned()
    {
        Setup2RecipesTest();

        var result = await _grocer.BreakdownItem(_firstItemId);

        Assert.Multiple(() =>
        {
            Assert.That(result.DistinctBy(i => i.RecipeId).Count, Is.EqualTo(2));
            Assert.That(result.DistinctBy(i => i.ItemId).Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentRecipeId = 1033;
        _recipes.Get(inexistentRecipeId).ReturnsNull();

        var result = await _grocer.BreakdownRecipeById(inexistentRecipeId);

        _recipes.Received(1).Get(inexistentRecipeId);
        Assert.That(result.GetIngredientsCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas1Ingredient_ThenReturn1()
    {
        const int existentRecipeId = 1033;
        var recipe = NewRecipe;
        recipe.AmountIngredient1 = 0;
        recipe.ItemIngredient1TargetId = 0;
        _recipes.Get(existentRecipeId).Returns(recipe);
        var activeIngredients = recipe.GetActiveIngredients();
        foreach (var ingredient in activeIngredients)
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        var expectedTotalIngredients = activeIngredients.GetIngredientsSum();

        var result = await _grocer.BreakdownRecipeById(existentRecipeId);

        _recipes.Received(1).Get(existentRecipeId);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(1));
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas2Ingredients_ThenReturn2()
    {
        const int existentRecipeId = 1033;
        var recipe = NewRecipe;
        var activeIngredients = recipe.GetActiveIngredients();
        foreach (var ingredient in activeIngredients)
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        var expectedTotalIngredients = activeIngredients.GetIngredientsSum();
        _recipes.Get(existentRecipeId).Returns(recipe);

        var result = await _grocer.BreakdownRecipeById(existentRecipeId);

        var resultTotalIngredients = result.GetIngredientsSum();
        _recipes.Received(1).Get(existentRecipeId);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.GreaterThanOrEqualTo(2));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas3Ingredients_ThenReturn3()
    {
        var recipe = NewRecipe;
        recipe.AmountIngredient2 = 5;
        recipe.ItemIngredient2TargetId = 753;
        _recipes.Get(recipe.Id).Returns(recipe);
        MockIngredientsToReturnEmpty(recipe.GetActiveIngredients());
        var activeIngredients = recipe.GetActiveIngredients();
        foreach (var ingredient in activeIngredients)
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
        var expectedIngredientsSum = activeIngredients.GetIngredientsSum();
        var expectedIngredientsCount = activeIngredients.GetIngredientsCount();

        var result = await _grocer.BreakdownRecipeById(recipe.Id);

        _recipes.Received(1).Get(recipe.Id);
        _recipes.Received(3).GetRecipesForItem(Arg.Is<int>(i => i > 0));
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedIngredientsSum));
            Assert.That(result.GetIngredientsCount(), Is.GreaterThanOrEqualTo(3));
            Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas2Levels_ThenReturnAllBrokenDown()
    {
        var firstRecipe = NewRecipe;
        firstRecipe.ItemIngredient0TargetId = _subItem0Id;
        firstRecipe.ItemIngredient1TargetId = _subItem1Id;
        var secondRecipe = new RecipePoco
        {
            Id = _secondRecipeId,
            ResultQuantity = 1,
            AmountIngredient0 = 7,
            ItemIngredient0TargetId = 9991,
            AmountIngredient1 = 2,
            ItemIngredient1TargetId = 8881,
            TargetItemId = _subItem1Id
        };
        SetUp2LevelsTest(firstRecipe, secondRecipe);
        var expectedIngredients =
            firstRecipe.AmountIngredient0
            + (
                firstRecipe.AmountIngredient1
                * (secondRecipe.AmountIngredient0 + secondRecipe.AmountIngredient1)
            );

        var result = await _grocer.BreakdownRecipe(firstRecipe);

        //Ensure we check for every item and its respective potential breakdowns
        _recipes.Received(1).Get(secondRecipe.Id);
        _recipes.Received(1).GetRecipesForItem(firstRecipe.ItemIngredient0TargetId);
        _recipes.Received(1).GetRecipesForItem(firstRecipe.ItemIngredient1TargetId);
        _recipes.Received(1).GetRecipesForItem(secondRecipe.ItemIngredient0TargetId);
        _recipes.Received(1).GetRecipesForItem(secondRecipe.ItemIngredient1TargetId);
        Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedIngredients));
    }

    private void SetUp2LevelsTest(RecipePoco firstRecipe, RecipePoco secondRecipe)
    {
        _recipes.Get(firstRecipe.Id).Returns(firstRecipe);
        _recipes.Get(secondRecipe.Id).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient0TargetId)
            .Returns([]);
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient1TargetId)
            .Returns([secondRecipe]);
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient0TargetId)
            .Returns([]);
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient1TargetId)
            .Returns([]);
    }

    private int Setup2LevelItemTest()
    {
        var firstRecipe = new RecipePoco
        {
            Id = _firstRecipeId,
            TargetItemId = _firstItemId,
            ResultQuantity = 1,
            ItemIngredient0TargetId = _subItem0Id,
            AmountIngredient0 = 2,
            ItemIngredient1TargetId = _subItem1Id,
            AmountIngredient1 = 3
        };
        var secondRecipe = new RecipePoco()
        {
            Id = _secondRecipeId,
            TargetItemId = _subItem1Id,
            ResultQuantity = 1,
            ItemIngredient0TargetId = _subItem2Id,
            AmountIngredient0 = 4
        };
        _recipes.Get(firstRecipe.Id).Returns(firstRecipe);
        _recipes.Get(secondRecipe.Id).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.TargetItemId)
            .Returns([firstRecipe]);
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient0TargetId)
            .Returns([]);
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient1TargetId)
            .Returns([secondRecipe]);
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient0TargetId)
            .Returns([]);
        return firstRecipe.AmountIngredient0
               + (firstRecipe.AmountIngredient1 * secondRecipe.AmountIngredient0);
    }

    private void Setup2RecipesTest()
    {
        var firstRecipe = new RecipePoco
        {
            Id = _firstRecipeId,
            TargetItemId = _firstItemId,
            ResultQuantity = 2,
            ItemIngredient0TargetId = _subItem0Id,
            AmountIngredient0 = 4,
        };
        var secondRecipe = new RecipePoco()
        {
            Id = _secondRecipeId,
            TargetItemId = _firstItemId,
            ResultQuantity = 3,
            ItemIngredient0TargetId = _subItem1Id,
            AmountIngredient0 = 5
        };
        _recipes.Get(firstRecipe.Id).Returns(firstRecipe);
        _recipes.Get(secondRecipe.Id).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.TargetItemId)
            .Returns(new List<RecipePoco>() { firstRecipe, secondRecipe });
        MockIngredientsToReturnEmpty(firstRecipe.GetIngredientsList());
        MockIngredientsToReturnEmpty(secondRecipe.GetIngredientsList());
    }

    private void MockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
    {
        foreach (var ingredient in ingredients)
            _recipes.GetRecipesForItem(ingredient.ItemId).Returns([]);
    }

    private RecipePoco SetupBreakingDown3ForItem(int itemId)
    {
        var recipe = Get3IngredientRecipe(itemId);
        _recipes.Get(recipe.Id).Returns(recipe);
        _recipes.GetRecipesForItem(itemId).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem0Id).Returns([]);
        _recipes.GetRecipesForItem(_subItem1Id).Returns([]);
        _recipes.GetRecipesForItem(_secondItemId).Returns([]);
        return recipe;
    }

    private static RecipePoco Get3IngredientRecipe(int itemId)
    {
        var recipe = NewRecipe;
        recipe.TargetItemId = itemId;
        recipe.ResultQuantity = 1;
        var subItem0 = new IngredientPoco(_subItem0Id, 10, recipe.Id);
        var subItem1 = new IngredientPoco(_subItem1Id, 4, recipe.Id);
        var subItem2 = new IngredientPoco(_secondItemId, 7, recipe.Id);
        recipe.ItemIngredient0TargetId = subItem0.ItemId;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetId = subItem1.ItemId;
        recipe.AmountIngredient1 = subItem1.Quantity;
        recipe.ItemIngredient2TargetId = subItem2.ItemId;
        recipe.AmountIngredient2 = subItem2.Quantity;
        return recipe;
    }

    private static RecipePoco NewRecipe =>
        new(
            true,
            true,
            _targetItemId,
            _firstRecipeId,
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