using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Crafting;

public class RecipeGrocerTests
{
    private RecipeGrocer _grocer;
    private IRecipeRepository _recipes;
    private IPriceRepository<PricePoco> _prices;
    private ILogger<RecipeGrocer> _logger;

    private static readonly int _firstRecipeID = 6044;
    private static readonly int _secondRecipeID = 2000;
    private static readonly int _firstItemID = 554;
    private static readonly int _secondItemID = 668;
    private static readonly int _subItem0ID = 4000;
    private static readonly int _subItem1ID = 4100;
    private static readonly int _subItem2ID = 4200;
    private static readonly int _targetItemID = 955;

    [SetUp]
    public void SetUp()
    {
        _recipes = Substitute.For<IRecipeRepository>();
        _prices = Substitute.For<IPriceRepository<PricePoco>>();
        _logger = Substitute.For<ILogger<RecipeGrocer>>();
        _grocer = new RecipeGrocer(_recipes, _logger);
    }

    [Test]
    public async Task GivenABreakdownItem_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentItemID = 1033;
        _recipes.GetRecipesForItem(inexistentItemID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(inexistentItemID);

        _recipes.Received(1).GetRecipesForItem(inexistentItemID);
        _recipes.DidNotReceiveWithAnyArgs().Get(inexistentItemID);
        Assert.That(result.GetIngredientsCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas1Ingredient_ThenReturn1()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var subItem1 = new IngredientPoco(_subItem0ID, 10, recipe.ID);
        recipe.AmountIngredient0 = subItem1.Quantity;
        recipe.ItemIngredient0TargetID = subItem1.ItemID;
        var expectedTotalIngredients = recipe.GetActiveIngredients().GetIngredientsSum();
        _recipes.Get(recipe.ID).Returns(recipe);
        _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem0ID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(itemID);

        _recipes.Received(1).GetRecipesForItem(itemID);
        _recipes.Received(1).GetRecipesForItem(_subItem0ID);
        _recipes.Received(1).Get(recipe.ID);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas2Ingredients_ThenReturn2()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var subItem0 = new IngredientPoco(_subItem0ID, 10, recipe.ID);
        var subItem1 = new IngredientPoco(_subItem1ID, 4, recipe.ID);
        recipe.ItemIngredient0TargetID = subItem0.ItemID;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetID = subItem1.ItemID;
        recipe.AmountIngredient1 = subItem1.Quantity;
        var expectedTotalIngredients = recipe.GetActiveIngredients().GetIngredientsSum();
        _recipes.Get(recipe.ID).Returns(recipe);
        _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem0ID).Returns(Array.Empty<RecipePoco>());
        _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(itemID);

        var resultTotalIngredients = result.GetIngredientsSum();
        _recipes.Received(1).GetRecipesForItem(itemID);
        _recipes.Received(1).GetRecipesForItem(_subItem0ID);
        _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        _recipes.Received(1).Get(recipe.ID);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenABreakdownItem_WhenItHas3Ingredients_ThenReturn3()
    {
        const int itemID = 2344;
        var recipe = SetupBreakingDown3ForItem(itemID);

        var result = await _grocer.BreakdownItem(itemID);

        _recipes.Received(1).GetRecipesForItem(itemID);
        _recipes.Received(1).GetRecipesForItem(_subItem0ID);
        _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        _recipes.Received(1).GetRecipesForItem(_secondItemID);
        _recipes.Received(1).Get(recipe.ID);
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

        var result = await _grocer.BreakdownItem(_firstItemID);

        _recipes.Received(1).GetRecipesForItem(_firstItemID);
        _recipes.Received(1).Get(_firstRecipeID);
        _recipes.Received(1).GetRecipesForItem(_subItem0ID);
        _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        _recipes.Received(1).Get(_secondRecipeID);
        _recipes.Received(1).GetRecipesForItem(_subItem2ID);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedIngredientsCount));
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownItem_WhenRecipeHasNullIngredients_ThenTheyAreIgnored()
    {
        var recipes = new List<RecipePoco> { null, null };
        _recipes.GetRecipesForItem(_firstItemID).Returns(recipes);

        var result = await _grocer.BreakdownItem(NewRecipe.ID);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenACallToBreakdownItem_When2RecipesExist_ThenIngredientsForBothAreReturned()
    {
        Setup2RecipesTest();

        var result = await _grocer.BreakdownItem(_firstItemID);

        Assert.Multiple(() =>
        {
            Assert.That(result.DistinctBy(i => i.RecipeID).Count, Is.EqualTo(2));
            Assert.That(result.DistinctBy(i => i.ItemID).Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentRecipeID = 1033;
        _recipes.Get(inexistentRecipeID).ReturnsNull();

        var result = await _grocer.BreakdownRecipeById(inexistentRecipeID);

        _recipes.Received(1).Get(inexistentRecipeID);
        Assert.That(result.GetIngredientsCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHasNullIngredients_ThenTheyAreIgnored()
    {
        var allIngredients = NewRecipe.GetIngredientsList().GetRange(0, 5);
        allIngredients.Add(null);
        allIngredients.Add(null);

        var result = await _grocer.BreakDownIngredientEntirely(allIngredients);

        Assert.Multiple(() =>
        {
            Assert.That(allIngredients.Count(i => i is null), Is.GreaterThan(0));
            Assert.That(allIngredients.Count(i => i is not null), Is.GreaterThan(0));
            Assert.That(result.Count(), Is.EqualTo(5));
            Assert.That(result.Any(i => i is null), Is.False);
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas1Ingredient_ThenReturn1()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        recipePoco.AmountIngredient1 = 0;
        recipePoco.ItemIngredient1TargetID = 0;
        _recipes.Get(existentRecipeID).Returns(recipePoco);
        var expectedTotalIngredients = recipePoco.GetActiveIngredients().GetIngredientsSum();

        var result = await _grocer.BreakdownRecipeById(existentRecipeID);

        _recipes.Received(1).Get(existentRecipeID);
        Assert.Multiple(() =>
        {
            Assert.That(result.GetIngredientsCount(), Is.EqualTo(1));
            Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedTotalIngredients));
        });
    }

    [Test]
    public async Task GivenACallToBreakdownRecipeById_WhenItHas2Ingredients_ThenReturn2()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        var expectedTotalIngredients = recipePoco.GetActiveIngredients().GetIngredientsSum();
        _recipes.Get(existentRecipeID).Returns(recipePoco);

        var result = await _grocer.BreakdownRecipeById(existentRecipeID);

        var resultTotalIngredients = result.GetIngredientsSum();
        _recipes.Received(1).Get(existentRecipeID);
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
        recipe.ItemIngredient2TargetID = 753;
        _recipes.Get(recipe.ID).Returns(recipe);
        MockIngredientsToReturnEmpty(recipe.GetActiveIngredients());
        var expectedIngredientsSum = recipe.GetActiveIngredients().GetIngredientsSum();
        var expectedIngredientsCount = recipe.GetActiveIngredients().GetIngredientsCount();

        var result = await _grocer.BreakdownRecipeById(recipe.ID);

        _recipes.Received(1).Get(recipe.ID);
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
        firstRecipe.ItemIngredient0TargetID = _subItem0ID;
        firstRecipe.ItemIngredient1TargetID = _subItem1ID;
        var secondRecipe = new RecipePoco
        {
            ID = _secondRecipeID,
            ResultQuantity = 1,
            AmountIngredient0 = 7,
            ItemIngredient0TargetID = 9991,
            AmountIngredient1 = 2,
            ItemIngredient1TargetID = 8881,
            TargetItemID = _subItem1ID
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
        _recipes.Received(1).Get(secondRecipe.ID);
        _recipes.Received(1).GetRecipesForItem(firstRecipe.ItemIngredient0TargetID);
        _recipes.Received(1).GetRecipesForItem(firstRecipe.ItemIngredient1TargetID);
        _recipes.Received(1).GetRecipesForItem(secondRecipe.ItemIngredient0TargetID);
        _recipes.Received(1).GetRecipesForItem(secondRecipe.ItemIngredient1TargetID);
        Assert.That(result.GetIngredientsSum(), Is.EqualTo(expectedIngredients));
    }

    private void SetUp2LevelsTest(RecipePoco firstRecipe, RecipePoco secondRecipe)
    {
        _recipes.Get(firstRecipe.ID).Returns(firstRecipe);
        _recipes.Get(secondRecipe.ID).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient0TargetID)
            .Returns(Array.Empty<RecipePoco>());
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient1TargetID)
            .Returns(new List<RecipePoco>() { secondRecipe });
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient0TargetID)
            .Returns(Array.Empty<RecipePoco>());
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient1TargetID)
            .Returns(Array.Empty<RecipePoco>());
    }

    private int Setup2LevelItemTest()
    {
        var firstRecipe = new RecipePoco
        {
            ID = _firstRecipeID,
            TargetItemID = _firstItemID,
            ResultQuantity = 1,
            ItemIngredient0TargetID = _subItem0ID,
            AmountIngredient0 = 2,
            ItemIngredient1TargetID = _subItem1ID,
            AmountIngredient1 = 3
        };
        var secondRecipe = new RecipePoco()
        {
            ID = _secondRecipeID,
            TargetItemID = _subItem1ID,
            ResultQuantity = 1,
            ItemIngredient0TargetID = _subItem2ID,
            AmountIngredient0 = 4
        };
        _recipes.Get(firstRecipe.ID).Returns(firstRecipe);
        _recipes.Get(secondRecipe.ID).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.TargetItemID)
            .Returns(new List<RecipePoco>() { firstRecipe });
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient0TargetID)
            .Returns(Array.Empty<RecipePoco>());
        _recipes
            .GetRecipesForItem(firstRecipe.ItemIngredient1TargetID)
            .Returns(new List<RecipePoco>() { secondRecipe });
        _recipes
            .GetRecipesForItem(secondRecipe.ItemIngredient0TargetID)
            .Returns(Array.Empty<RecipePoco>());
        return firstRecipe.AmountIngredient0
            + (firstRecipe.AmountIngredient1 * secondRecipe.AmountIngredient0);
    }

    private void Setup2RecipesTest()
    {
        var firstRecipe = new RecipePoco
        {
            ID = _firstRecipeID,
            TargetItemID = _firstItemID,
            ResultQuantity = 2,
            ItemIngredient0TargetID = _subItem0ID,
            AmountIngredient0 = 4,
        };
        var secondRecipe = new RecipePoco()
        {
            ID = _secondRecipeID,
            TargetItemID = _firstItemID,
            ResultQuantity = 3,
            ItemIngredient0TargetID = _subItem1ID,
            AmountIngredient0 = 5
        };
        _recipes.Get(firstRecipe.ID).Returns(firstRecipe);
        _recipes.Get(secondRecipe.ID).Returns(secondRecipe);
        _recipes
            .GetRecipesForItem(firstRecipe.TargetItemID)
            .Returns(new List<RecipePoco>() { firstRecipe, secondRecipe });
        MockIngredientsToReturnEmpty(firstRecipe.GetIngredientsList());
        MockIngredientsToReturnEmpty(secondRecipe.GetIngredientsList());
    }

    private void MockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
    {
        foreach (var ingredient in ingredients)
            _recipes.GetRecipesForItem(ingredient.ItemID).Returns(Array.Empty<RecipePoco>());
    }

    private RecipePoco SetupBreakingDown3ForItem(int itemID)
    {
        var recipe = Get3IngredientRecipe(itemID);
        _recipes.Get(recipe.ID).Returns(recipe);
        _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem0ID).Returns(Array.Empty<RecipePoco>());
        _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());
        _recipes.GetRecipesForItem(_secondItemID).Returns(Array.Empty<RecipePoco>());
        return recipe;
    }

    private static RecipePoco Get3IngredientRecipe(int itemID)
    {
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var subItem0 = new IngredientPoco(_subItem0ID, 10, recipe.ID);
        var subItem1 = new IngredientPoco(_subItem1ID, 4, recipe.ID);
        var subItem2 = new IngredientPoco(_secondItemID, 7, recipe.ID);
        recipe.ItemIngredient0TargetID = subItem0.ItemID;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetID = subItem1.ItemID;
        recipe.AmountIngredient1 = subItem1.Quantity;
        recipe.ItemIngredient2TargetID = subItem2.ItemID;
        recipe.AmountIngredient2 = subItem2.Quantity;
        return recipe;
    }

    private static RecipePoco NewRecipe =>
        new(
            true,
            true,
            _targetItemID,
            _firstRecipeID,
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
