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

public class RecipeGrocerTests
{
    private readonly IRecipeRepository _recipes = Substitute.For<IRecipeRepository>();
    private readonly IPriceRepository _prices = Substitute.For<IPriceRepository>();
    private readonly RecipeGrocer _grocer;
    private readonly ILogger<RecipeGrocer> _log =
        NullLoggerFactory.Instance.CreateLogger<RecipeGrocer>();

    private static readonly int _worldID = 34; // Brynnhildr
    private static readonly int _firstItemID = 554;
    private static readonly int _secondItemID = 668;
    private static readonly int _secondRecipeID = 2000;
    private static readonly int _subItem1ID = 4100;
    private static readonly int _subItem2ID = 4200;
    private static readonly int _recipeID = 6044;
    private static readonly int _targetItemID = 955;

    public RecipeGrocerTests()
    {
        _grocer = new RecipeGrocer(_recipes, _log);
    }

    [SetUp]
    public void SetUp() { }

    [TearDown]
    public void TearDown()
    {
        _recipes.ClearReceivedCalls();
        _prices.ClearReceivedCalls();
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownAnItem_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentItemID = 1033;
        _recipes.GetRecipesForItem(inexistentItemID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(inexistentItemID);

        await _recipes.Received(1).GetRecipesForItem(inexistentItemID);
        await _recipes.DidNotReceiveWithAnyArgs().Get(inexistentItemID);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownAnItem_WhenItHas1Ingredient_ThenReturn1()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        var recipeID = recipe.ID;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var subItem1 = new IngredientPoco(_subItem1ID, 10, recipeID);
        recipe.AmountIngredient0 = subItem1.Quantity;
        recipe.ItemIngredient0TargetID = subItem1.ItemID;
        var expectedTotalIngredients = recipe.Ingredients().Select(x => x.Quantity).Sum();
        _recipes.Get(recipeID).Returns(recipe);
        _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(itemID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        await _recipes.Received(1).GetRecipesForItem(itemID);
        await _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        await _recipes.Received(1).Get(recipeID);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        Assert.That(recipe.Ingredients(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownAnItem_WhenItHas2Ingredients_ThenReturn2()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var recipeID = recipe.ID;
        var subItem0 = new IngredientPoco(_subItem1ID, 10, recipeID);
        var subItem1 = new IngredientPoco(_subItem2ID, 4, recipeID);
        recipe.ItemIngredient0TargetID = subItem0.ItemID;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetID = subItem1.ItemID;
        recipe.AmountIngredient1 = subItem1.Quantity;
        var expectedTotalIngredients = recipe.Ingredients().Select(x => x.Quantity).Sum();
        _recipes.Get(recipeID).Returns(recipe);
        _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
        _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());
        _recipes.GetRecipesForItem(_subItem2ID).Returns(Array.Empty<RecipePoco>());

        var result = await _grocer.BreakdownItem(itemID);

        var resultTotalIngredients = result.Select(x => x?.Quantity).Sum();
        await _recipes.Received(1).GetRecipesForItem(itemID);
        await _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        await _recipes.Received(1).GetRecipesForItem(_subItem2ID);
        await _recipes.Received(1).Get(recipeID);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        Assert.That(recipe.Ingredients(), Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownAnItem_WhenItHas3Ingredients_ThenReturn3()
    {
        const int itemID = 2344;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var recipeID = recipe.ID;
        var subItem0 = new IngredientPoco(_subItem1ID, 10, recipeID);
        var subItem1 = new IngredientPoco(_subItem2ID, 4, recipeID);
        var subItem2 = new IngredientPoco(_secondItemID, 7, recipeID);
        recipe.ItemIngredient0TargetID = subItem0.ItemID;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetID = subItem1.ItemID;
        recipe.AmountIngredient1 = subItem1.Quantity;
        recipe.ItemIngredient2TargetID = subItem2.ItemID;
        recipe.AmountIngredient2 = subItem2.Quantity;
        var expectedTotalIngredients = recipe.Ingredients().Select(x => x.Quantity).Sum();
        SetupBreakingDown3ForItem(itemID, recipe, recipeID);

        var result = await _grocer.BreakdownItem(itemID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        await _recipes.Received(1).GetRecipesForItem(itemID);
        await _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        await _recipes.Received(1).GetRecipesForItem(_subItem2ID);
        await _recipes.Received(1).GetRecipesForItem(_secondItemID);
        await _recipes.Received(1).Get(recipeID);
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        Assert.That(recipe.Ingredients(), Has.Count.EqualTo(3));

        void SetupBreakingDown3ForItem(int itemID, RecipePoco recipe, int recipeID)
        {
            _recipes.Get(recipeID).Returns(recipe);
            _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
            _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());
            _recipes.GetRecipesForItem(_subItem2ID).Returns(Array.Empty<RecipePoco>());
            _recipes.GetRecipesForItem(_secondItemID).Returns(Array.Empty<RecipePoco>());
        }
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownAnItem_WhenItHas2Levels_ThenReturnAllBrokenDown()
    {
        const int itemID = 2344;
        const int subRecipeID = 8844;
        var recipe = NewRecipe;
        recipe.TargetItemID = itemID;
        recipe.ResultQuantity = 1;
        var recipeID = recipe.ID;
        var subItem0 = new IngredientPoco(_subItem1ID, 10, recipeID);
        var subItem1 = new IngredientPoco(_subItem2ID, 4, recipeID);
        recipe.ItemIngredient0TargetID = subItem0.ItemID;
        recipe.AmountIngredient0 = subItem0.Quantity;
        recipe.ItemIngredient1TargetID = subItem1.ItemID;
        recipe.AmountIngredient1 = subItem1.Quantity;

        var subSubItem0 = new IngredientPoco(_secondItemID, 7, subRecipeID);
        var subRecipe = new RecipePoco()
        {
            ID = subRecipeID,
            ItemIngredient0TargetID = subSubItem0.ItemID,
            AmountIngredient0 = subSubItem0.Quantity,
        };
        Setup2LevelItemTest(itemID, subRecipeID, recipe, recipeID, subRecipe);
        var expectedIngredientsSum =
            recipe.Ingredients().First().Quantity + subRecipe.Ingredients().First().Quantity;

        var result = await _grocer.BreakdownItem(itemID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        await _recipes.Received(1).GetRecipesForItem(itemID);
        await _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        await _recipes.Received(1).GetRecipesForItem(_subItem2ID);
        await _recipes.Received(1).GetRecipesForItem(_secondItemID);
        await _recipes.Received(1).Get(recipeID);
        await _recipes.Received(1).Get(subRecipeID);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedIngredientsSum));
        Assert.That(recipe.Ingredients(), Has.Count.EqualTo(2));

        void Setup2LevelItemTest(
            int itemID,
            int subRecipeID,
            RecipePoco recipe,
            int recipeID,
            RecipePoco subRecipe
        )
        {
            _recipes.Get(recipeID).Returns(recipe);
            _recipes.Get(subRecipeID).Returns(subRecipe);
            _recipes.GetRecipesForItem(itemID).Returns(new List<RecipePoco>() { recipe });
            _recipes.GetRecipesForItem(_subItem1ID).Returns(Array.Empty<RecipePoco>());
            _recipes.GetRecipesForItem(_subItem2ID).Returns(new List<RecipePoco>() { subRecipe });
            _recipes.GetRecipesForItem(_secondItemID).Returns(Array.Empty<RecipePoco>());
        }
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownARecipe_WhenRecipeDoesNotExist_ThenReturnEmpty()
    {
        const int inexistentRecipeID = 1033;
        _recipes.Get(inexistentRecipeID).ReturnsNull();

        var result = await _grocer.BreakdownRecipe(inexistentRecipeID);

        await _recipes.Received(1).Get(inexistentRecipeID);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownARecipe_WhenItHas1Ingredient_ThenReturn1()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        recipePoco.AmountIngredient1 = 0;
        recipePoco.ItemIngredient1TargetID = 0;
        var expectedTotalIngredients = recipePoco.Ingredients().Select(x => x.Quantity).Sum();
        _recipes.Get(existentRecipeID).Returns(recipePoco);
        Assume.That(recipePoco.Ingredients().Count, Is.EqualTo(1));

        var result = await _grocer.BreakdownRecipe(existentRecipeID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        await _recipes.Received(1).Get(existentRecipeID);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        Assert.That(recipePoco.Ingredients(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownARecipe_WhenItHas2Ingredients_ThenReturn2()
    {
        const int existentRecipeID = 1033;
        var recipePoco = NewRecipe;
        var expectedTotalIngredients = recipePoco.Ingredients().Select(x => x.Quantity).Sum();
        _recipes.Get(existentRecipeID).Returns(recipePoco);
        Assume.That(recipePoco.Ingredients().Count, Is.EqualTo(2));

        var result = await _grocer.BreakdownRecipe(existentRecipeID);

        var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
        await _recipes.Received(1).Get(existentRecipeID);
        Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
        Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredients));
        Assert.That(recipePoco.Ingredients(), Has.Count.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownARecipe_WhenItHas3Ingredients_ThenReturn3()
    {
        var recipe = NewRecipe;
        var recipeID = recipe.ID;

        recipe.Ingredients().Add(new IngredientPoco(753, 5, recipeID));
        _recipes.Get(recipeID).Returns(recipe);
        MockIngredientsToReturnEmpty(recipe.Ingredients());
        var expectedIngredientsSum = recipe.Ingredients().Select(x => x.Quantity).Sum();
        var expectedIngredientsCount = recipe.Ingredients().Count;
        Assume.That(recipe.Ingredients().Count, Is.EqualTo(3));

        var result = await _grocer.BreakdownRecipe(recipeID);

        var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
        var resultIngredientsCount = result.Count();
        await _recipes.Received(1).Get(recipeID);
        await _recipes.ReceivedWithAnyArgs(3).GetRecipesForItem(Arg.Any<int>());
        Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
        Assert.That(resultIngredientsCount, Is.GreaterThanOrEqualTo(3));
        Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
    }

    [Test]
    public async Task GivenARecipeGrocer_WhenBreakingDownARecipe_WhenItHas2Levels_ThenReturnAllBrokenDown()
    {
        var firstRecipe = NewRecipe;
        var firstRecipeID = firstRecipe.ID;
        var subItem1 = GetNewPrice;
        subItem1.ItemID = _subItem1ID;
        var subItem2 = GetNewPrice;
        subItem2.ItemID = _subItem2ID;
        var secondRecipe = GetSecondRecipe(subItem1, subItem2);

        var expectedIngredients = GetExpectedIngredientsFromRecipes(firstRecipe, secondRecipe);
        var expectedIngredientsSum = expectedIngredients.Select(x => x.Quantity).Sum();
        var expectedIngredientsCount = expectedIngredients.Count;
        Setup2LevelsRecipeTest(firstRecipe, secondRecipe);
        MockIngredientsToReturnEmpty(secondRecipe.Ingredients());
        VerifyAssumptions(
            firstRecipe,
            secondRecipe,
            expectedIngredientsSum,
            expectedIngredientsCount
        );

        var result = await _grocer.BreakdownRecipe(firstRecipeID);

        //Ensure we check for every item and its respective potential breakdowns
        var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
        var resultIngredientsCount = result.Count();
        await _recipes.Received(1).Get(firstRecipeID);
        await _recipes.Received(1).Get(_secondRecipeID);
        await _recipes.Received(1).GetRecipesForItem(_firstItemID);
        await _recipes.Received(1).GetRecipesForItem(_secondItemID);
        await _recipes.Received(1).GetRecipesForItem(_subItem1ID);
        await _recipes.Received(1).GetRecipesForItem(_subItem2ID);
        Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
        Assert.That(resultIngredientsCount, Is.EqualTo(expectedIngredientsCount));

        void Setup2LevelsRecipeTest(RecipePoco firstRecipe, RecipePoco secondRecipe)
        {
            _recipes.Get(firstRecipe.ID).Returns(firstRecipe);
            _recipes.Get(secondRecipe.ID).Returns(secondRecipe);
            _recipes.GetRecipesForItem(_firstItemID).Returns(Array.Empty<RecipePoco>());
            _recipes
                .GetRecipesForItem(_secondItemID)
                .Returns(new List<RecipePoco>() { secondRecipe });
        }

        void VerifyAssumptions(
            RecipePoco firstRecipe,
            RecipePoco secondRecipe,
            int expectedIngredientsSum,
            int expectedIngredientsCount
        )
        {
            Assume.That(firstRecipe.Ingredients().Count, Is.EqualTo(2));
            Assume.That(secondRecipe.Ingredients().Count, Is.EqualTo(2));
            Assume.That(expectedIngredientsCount, Is.EqualTo(3));
            Assume.That(secondRecipe.Ingredients().Count, Is.GreaterThan(1));
            Assume.That(firstRecipe.Ingredients().Count, Is.GreaterThan(1));
            var unBrokendownSum = firstRecipe.Ingredients().Select(i => i.Quantity).Sum();
            Assume.That(expectedIngredientsSum, Is.GreaterThan(unBrokendownSum));
        }
    }

    private void MockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
    {
        foreach (var ingredient in ingredients)
            _recipes.GetRecipesForItem(ingredient.ItemID).Returns(Array.Empty<RecipePoco>());
    }

    private static List<IngredientPoco> GetExpectedIngredientsFromRecipes(
        RecipePoco firstRecipe,
        RecipePoco secondRecipe
    )
    {
        var secondRecipeIngredients = secondRecipe.Ingredients();
        var expectedIngredients = new List<IngredientPoco>(secondRecipeIngredients);
        Assume.That(expectedIngredients, Is.Not.Empty);

        var firstRecipeIngredient = firstRecipe.Ingredients().FirstOrDefault()!;
        Assume.That(firstRecipeIngredient, Is.Not.Null);

        expectedIngredients.Add(new IngredientPoco(firstRecipeIngredient));
        Assume.That(expectedIngredients.Count, Is.GreaterThan(secondRecipeIngredients.Count));
        return expectedIngredients;
    }

    private static RecipePoco GetSecondRecipe(PricePoco subItem1, PricePoco subItem2)
    {
        var recipe = NewRecipe;
        recipe.ID = _secondRecipeID;
        recipe.TargetItemID = _secondItemID;

        var ingredient1 = new IngredientPoco(subItem1.ItemID, 6, _secondRecipeID);
        var ingredient2 = new IngredientPoco(subItem2.ItemID, 7, _secondRecipeID);
        recipe.ItemIngredient0TargetID = ingredient1.ItemID;
        recipe.AmountIngredient0 = ingredient1.Quantity;
        recipe.ItemIngredient1TargetID = ingredient2.ItemID;
        recipe.AmountIngredient1 = ingredient2.Quantity;
        return recipe;
    }

    private static PricePoco GetNewPrice =>
        new(1, _worldID, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);

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
