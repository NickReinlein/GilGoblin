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
        private CraftingCalculator _calc = new CraftingCalculator();
        private IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
        private IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
        private ILogger _log = Substitute.For<ILogger>();

        private MarketDataPoco? _poco;
        private MarketDataPoco[]? _pocos;

        private int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        private const int WORLD_ID = 34; // Brynnhildr

        [SetUp]
        public void setUp()
        {
            _poco = _getMarketData();
            _pocos = new MarketDataPoco[] { _poco };

            _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway, _log);
        }

        [TearDown]
        public void tearDown()
        {
            _recipeGateway.ClearReceivedCalls();
            _marketDataGateway.ClearReceivedCalls();
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesNotExist_ThenReturnErrorCost()
        {
            int inexistentItemID = -200;
            _marketDataGateway
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>())
                .ReturnsForAnyArgs(Array.Empty<MarketDataPoco>());

            var result = _calc.CalculateCraftingCost(WORLD_ID, inexistentItemID);

            _marketDataGateway
                .ReceivedWithAnyArgs(1)
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>());
            //.Received().Error(Arg.Any<string>());
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesExist_ThenReturnCraftingCost()
        {
            const int existentRecipeID = 1033;
            MarketDataPoco goodPoco = _getMarketData();
            _marketDataGateway
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>())
                .ReturnsForAnyArgs(new List<MarketDataPoco>() { goodPoco });

            var result = _calc.CalculateCraftingCost(WORLD_ID, existentRecipeID);

            _marketDataGateway
                .ReceivedWithAnyArgs(1)
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>());
            Assert.That(result, Is.EqualTo(goodPoco.averageSold));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Ingredients_ThenReturn2()
        {
            const int existentRecipeID = 1033;
            var recipePoco = _getRecipe();
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
            var recipePoco = _getRecipe();
            var recipeID = recipePoco.recipeID;
            recipePoco.ingredients.Add(new pocos.IngredientPoco(753, 5, recipeID));
            var expectedTotalIngredientsCount = recipePoco.ingredients
                .Select(x => x.Quantity)
                .Sum();
            _recipeGateway.GetRecipe(recipeID).Returns(recipePoco);

            var result = _calc.BreakdownRecipe(recipeID);

            var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
            _recipeGateway.Received(1).GetRecipe(recipeID);
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(3));
            Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
            Assert.That(recipePoco.ingredients.Count(), Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Levels_ThenReturnAllBrokenDown()
        {
            const int secondRecipeID = 2000;
            const int secondItemID = 885;

            var secondItem = new MarketDataPoco(_poco);
            secondItem.name = "Iron Ore";
            secondItem.itemID = secondItemID;
            var secondRecipeIngredient = new IngredientPoco(secondItem.itemID, 2, secondRecipeID);

            RecipeFullPoco firstRecipe = _getRecipe();
            var firstRecipeID = firstRecipe.recipeID;
            var firstItem = firstRecipe.ingredients.First();
            var secondRecipe = new RecipeFullPoco(firstRecipe);
            secondRecipe.ingredients = new List<IngredientPoco>() { secondRecipeIngredient };
            secondRecipe.recipeID = secondRecipeID;

            _recipeGateway.GetRecipe(firstRecipeID).Returns(firstRecipe);
            _recipeGateway.GetRecipesForItem(firstItem.ItemID).Returns(new List<RecipeFullPoco>() { secondRecipe });
            _recipeGateway.GetRecipe(firstItem.RecipeID).Returns(secondRecipe);

            var result = _calc.BreakdownRecipe(firstRecipeID);

            //Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
            _recipeGateway.Received(1).GetRecipe(firstRecipeID);
            _recipeGateway.Received(1).GetRecipe(firstItem.RecipeID);
            //_recipeGateway.Received(1).GetRecipesForItem(firstItem.ItemID);

            Assert.That(firstRecipe.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
        }

        private static MarketDataPoco _getMarketData()
        {
            return new MarketDataPoco(1, 1, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);
        }

        private static RecipeFullPoco _getRecipe()
        {
            return new RecipeFullPoco(
                true,
                true,
                955,
                6044,
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
                554,
                668,
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
    }
}
