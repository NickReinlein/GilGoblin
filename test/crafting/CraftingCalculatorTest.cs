using GilGoblin.pocos;
using GilGoblin.web;
using GilGoblin.crafting;
using NSubstitute;
using NUnit.Framework;
using Serilog;

namespace GilGoblin.Test.crafting
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
        private IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
        private ILogger _log = Substitute.For<ILogger>();

        private MarketDataPoco? _poco;
        private MarketDataPoco[]? _pocos;

        private int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        private const int WORLD_ID = 34; // Brynnhildr
        private const int firstItemID = 554;
        private const int secondItemID = 668;
        public const int secondRecipeID = 2000;
        public const int subItem1ID = 4100;
        public const int subItem2ID = 4200;
        private CraftingCalculator _calc;

        [SetUp]
        public void setUp()
        {
            _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway, _log);
            _poco = _getNewMarketData();
            _pocos = new MarketDataPoco[] { _poco };
        }

        [TearDown]
        public void tearDown()
        {
            _recipeGateway.ClearReceivedCalls();
            _marketDataGateway.ClearReceivedCalls();
            _log.ClearReceivedCalls();
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForItem_WhenNoRecipesExist_ThenReturnErrorCost()
        {
            int inexistentItemID = -200;
            _recipeGateway
                .GetRecipesForItem(inexistentItemID)
                .Returns(Array.Empty<RecipePoco>());

            var result = _calc.CalculateCraftingCostForItem(WORLD_ID, inexistentItemID);

            _recipeGateway
                .Received(1)
                .GetRecipesForItem(inexistentItemID);
            _marketDataGateway.DidNotReceiveWithAnyArgs().GetMarketDataItems(default, default);
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForItem_WhenARecipeExists_ThenReturnCraftingCost()
        {
            const int existentItemID = 1033;
            var goodPoco = _getNewMarketData();
            var recipe =  _getNewRecipe();
            _recipeGateway
                .GetRecipesForItem(existentItemID)
                .Returns(new List<RecipePoco>() { recipe });
            _marketDataGateway
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>())
                .ReturnsForAnyArgs(Array.Empty<MarketDataPoco>());

            var result = _calc.CalculateCraftingCostForItem(WORLD_ID, existentItemID);

            _recipeGateway
                .Received(1)
                .GetRecipesForItem(existentItemID);
            _marketDataGateway
                .Received(1)
                .GetMarketDataItems(Arg.Any<int>(), Arg.Any<IEnumerable<int>>());
            Assert.That(result, Is.LessThan(int.MaxValue));            
            // todo: later we change this to an expected crafting value            
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas1Ingredient_ThenReturn1()
        {
            const int existentRecipeID = 1033;
            var recipePoco = _getNewRecipe();
            recipePoco.ingredients = new List<IngredientPoco>(){ recipePoco.ingredients.First()};
            var expectedTotalIngredientsCount = recipePoco.ingredients
                .Select(x => x.Quantity)
                .Sum();
            _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

            var result = _calc.BreakdownRecipe(existentRecipeID);

            var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
            _recipeGateway.Received(1).GetRecipe(existentRecipeID);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipePoco.ingredients.Count(), Is.EqualTo(1));
        }        

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Ingredients_ThenReturn2()
        {
            const int existentRecipeID = 1033;
            var recipePoco = _getNewRecipe();
            var expectedTotalIngredientsCount = recipePoco.ingredients
                .Select(x => x.Quantity)
                .Sum();
            _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

            var result = _calc.BreakdownRecipe(existentRecipeID);

            var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
            _recipeGateway.Received(1).GetRecipe(existentRecipeID);
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipePoco.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas3Ingredients_ThenReturn3()
        {
            var recipe = _getNewRecipe();
            var recipeID = recipe.recipeID;

            recipe.ingredients.Add(new pocos.IngredientPoco(753, 5, recipeID));
            _recipeGateway.GetRecipe(recipeID).Returns(recipe);
            _mockIngredientsToReturnEmpty(recipe.ingredients);
            var expectedIngredientsSum = recipe.ingredients.Select(x => x.Quantity).Sum();
            var expectedIngredientsCount = recipe.ingredients.Count;

            var result = _calc.BreakdownRecipe(recipeID);

            var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
            var resultIngredientsCount = result.Count();
            _recipeGateway.Received(1).GetRecipe(recipeID);
            _recipeGateway.ReceivedWithAnyArgs(3).GetRecipesForItem(Arg.Any<int>());
            Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
            Assert.That(resultIngredientsCount, Is.GreaterThanOrEqualTo(3));
            Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Levels_ThenReturnAllBrokenDown()
        {
            var firstRecipe = _getNewRecipe();
            var firstRecipeID = firstRecipe.recipeID;
            Assume.That(firstRecipe.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            var subItem1 = _getNewMarketData();
            subItem1.itemID = subItem1ID;
            var subItem2 = _getNewMarketData();
            subItem2.itemID = subItem2ID;
            var secondRecipe = _getSecondRecipe(subItem1, subItem2);
            Assume.That(secondRecipe.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            var expectedIngredients = _getExpectedIngredientsFromRecipes(firstRecipe, secondRecipe);
            var expectedIngredientsSum = expectedIngredients.Select(x => x.Quantity).Sum();
            var expectedIngredientsCount = expectedIngredients.Count;
            Assume.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
            _mockRecipeGatewayForTwoRecipes(firstRecipe, secondRecipe);

            var result = _calc.BreakdownRecipe(firstRecipeID);

            //Ensure we check for every item and its respective potential breakdowns
            var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
            var resultIngredientsCount = result.Count();
            _recipeGateway.Received(1).GetRecipe(firstRecipeID);
            _recipeGateway.Received(1).GetRecipe(secondRecipeID);
            _recipeGateway.Received(1).GetRecipesForItem(firstItemID);
            _recipeGateway.Received(1).GetRecipesForItem(secondItemID);
            _recipeGateway.Received(1).GetRecipesForItem(subItem1ID);
            _recipeGateway.Received(1).GetRecipesForItem(subItem2ID);
            Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
            Assert.That(resultIngredientsCount, Is.EqualTo(expectedIngredientsCount));
        }

        private void _mockRecipeGatewayForTwoRecipes(RecipePoco firstRecipe, RecipePoco secondRecipe)
        {
            Assume.That(secondRecipe.ingredients.Count, Is.GreaterThan(1));
            Assume.That(firstRecipe.ingredients.Count, Is.GreaterThan(1));
            _recipeGateway.GetRecipe(firstRecipe.recipeID).Returns(firstRecipe);
            _recipeGateway.GetRecipe(secondRecipe.recipeID).Returns(secondRecipe);
            _recipeGateway.GetRecipesForItem(firstItemID).Returns(Array.Empty<RecipePoco>());
            _recipeGateway.GetRecipesForItem(secondItemID).Returns(new List<RecipePoco>() { secondRecipe });
            _mockIngredientsToReturnEmpty(secondRecipe.ingredients);
        }

        private void _mockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
        {
            foreach (var ingredient in ingredients)
                _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(Array.Empty<RecipePoco>());
        }

        private List<IngredientPoco> _getExpectedIngredientsFromRecipes(RecipePoco firstRecipe, RecipePoco secondRecipe)
        {
            var secondRecipeIngredients = secondRecipe.ingredients;
            List<IngredientPoco> expectedIngredients = new List<IngredientPoco>(secondRecipeIngredients);
            Assume.That(expectedIngredients, Is.Not.Empty);

            IngredientPoco firstRecipeIngredient = firstRecipe.ingredients.FirstOrDefault()!;
            Assume.That(firstRecipeIngredient, Is.Not.Null);

            expectedIngredients.Add(new IngredientPoco(firstRecipeIngredient));
            Assume.That(expectedIngredients.Count, Is.GreaterThan(secondRecipeIngredients.Count));
            return expectedIngredients;
        }

        private RecipePoco _getSecondRecipe(MarketDataPoco subItem1, MarketDataPoco subItem2)
        {
            RecipePoco recipe = _getNewRecipe();
            recipe.recipeID = secondRecipeID;
            recipe.targetItemID = secondItemID;

            var ingredient1 = new IngredientPoco(subItem1.itemID, 6, secondRecipeID);
            var ingredient2 = new IngredientPoco(subItem2.itemID, 7, secondRecipeID);
            recipe.ingredients = new List<IngredientPoco>() { ingredient1, ingredient2 };

            return recipe;
        }

        private MarketDataPoco _getNewMarketData()
        {
            return new MarketDataPoco(1, 1, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);
        }

        private RecipePoco _getNewRecipe()
        {
            return new RecipePoco(
                true, true, 955, 6044, 254, 1, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, firstItemID, secondItemID, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
