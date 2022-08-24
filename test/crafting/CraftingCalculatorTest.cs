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
            _log.ClearReceivedCalls();
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

        // [Test]
        // public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas3Ingredients_ThenReturn3()
        // {
        //     var recipePoco = _getNewRecipe();
        //     var recipeID = recipePoco.recipeID;

        //     recipePoco.ingredients.Add(new pocos.IngredientPoco(753, 5, recipeID));
        //     _recipeGateway.GetRecipe(recipeID).Returns(recipePoco);
        //     var expectedIngredientsSum = recipePoco.ingredients.Select(x => x.Quantity).Sum();
        //     var expectedIngredientsCount = recipePoco.ingredients.Count;

        //     var result = _calc.BreakdownRecipe(recipeID);

        //     var resultIngredientsSum = result.Select(x => x.Quantity).Sum();
        //     var resultIngredientsCount = result.Count();
        //     _recipeGateway.Received(1).GetRecipe(recipeID);
        //     _recipeGateway.ReceivedWithAnyArgs(3).GetRecipesForItem(Arg.Any<int>());
        //     Assert.That(resultIngredientsSum, Is.EqualTo(expectedIngredientsSum));
        //     Assert.That(resultIngredientsCount, Is.GreaterThanOrEqualTo(3));
        //     Assert.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
        // }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Levels_ThenReturnAllBrokenDown()
        {
            const int secondRecipeID = 2000;
            var firstRecipe = _getNewRecipe();
            var firstRecipeID = firstRecipe.recipeID;
            var firstItemID = firstRecipe.ingredients[0].ItemID;
            var secondItemID = firstRecipe.ingredients[1].ItemID; ;

            var subItem1 = _getMarketData();
            subItem1.name = "Iron Ore";
            subItem1.itemID = secondItemID + 3455;

            var subItem2 = _getMarketData();
            subItem2.name = "Coal";
            subItem2.itemID = secondItemID + 3844;

            var secondRecipe = _getNewRecipe();
            var secondRecipeIngredient1 = new IngredientPoco(subItem1.itemID, 6, secondRecipeID);
            var secondRecipeIngredient2 = new IngredientPoco(subItem2.itemID, 7, secondRecipeID);
            secondRecipe.ingredients = new List<IngredientPoco>() { secondRecipeIngredient1, secondRecipeIngredient2 };
            secondRecipe.recipeID = secondRecipeID;
            secondRecipe.targetItemID = secondItemID;

            _recipeGateway.GetRecipe(firstRecipeID).Returns(firstRecipe);
            _recipeGateway.GetRecipesForItem(firstItemID).Returns(Array.Empty<RecipePoco>());
            _recipeGateway.GetRecipesForItem(secondItemID).Returns(new List<RecipePoco>() { secondRecipe });
            _recipeGateway.GetRecipesForItem(subItem1.itemID).Returns(Array.Empty<RecipePoco>());
            _recipeGateway.GetRecipesForItem(subItem2.itemID).Returns(Array.Empty<RecipePoco>());

            var result = _calc.BreakdownRecipe(firstRecipeID);

            Assert.That(firstRecipe.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(secondRecipe.ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            _recipeGateway.Received(1).GetRecipe(firstRecipeID);
            _recipeGateway.Received(1).GetRecipesForItem(firstItemID);
            _recipeGateway.Received(1).GetRecipesForItem(secondItemID);
            _recipeGateway.Received(1).GetRecipe(secondRecipeID);
            // _recipeGateway.Received(1).GetRecipesForItem(subItem1.itemID);
            // _recipeGateway.Received(1).GetRecipesForItem(subItem2.itemID);
        }

        private static MarketDataPoco _getMarketData()
        {
            return new MarketDataPoco(1, 1, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);
        }

        private static RecipePoco _getNewRecipe()
        {
            return new RecipePoco(
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
