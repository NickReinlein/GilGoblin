using GilGoblin.pocos;
using GilGoblin.web;
using GilGoblin.crafting;
using NSubstitute;
using NUnit.Framework;
using Serilog;
using System.Linq.Expressions;

namespace GilGoblin.Test.crafting
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private readonly IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
        private readonly IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
        private readonly ILogger _log = Substitute.For<ILogger>();

        private MarketDataPoco? _poco;
        private MarketDataPoco[]? _pocos;
        private CraftingCalculator? _calc;

        public int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        public const int WORLD_ID = 34; // Brynnhildr
        private const int firstItemID = 554;
        private const int secondItemID = 668;
        private const int secondRecipeID = 2000;
        private const int subItem1ID = 4100;
        private const int subItem2ID = 4200;
        private const int recipeID = 6044;
        private const int targetItemID = 955;

        [SetUp]
        public void SetUp()
        {
            _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway, _log);
            _poco = _getNewMarketData();
            _pocos = new MarketDataPoco[] { _poco };
        }

        [TearDown]
        public void TearDown()
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

            var result = _calc!.CalculateCraftingCostForItem(WORLD_ID, inexistentItemID);

            _recipeGateway.Received(1).GetRecipesForItem(inexistentItemID);
            _marketDataGateway.DidNotReceiveWithAnyArgs()
                              .GetMarketDataItems(default, default!);
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForItem_WhenARecipeExists_ThenReturnCraftingCost()
        {
            const int itemID = 1;
            const int ingredientID = 2;
            var market = _getNewMarketData();
            market.ItemID = itemID;
            var recipe = _getNewRecipe();
            recipe.TargetItemID = itemID;
            recipe.ResultQuantity = 1;

            var ingredient = new IngredientPoco(recipe.Ingredients.First());
            ingredient.Quantity = 10;
            ingredient.ItemID = ingredientID;
            var ingredientMarket = _getNewMarketData();
            ingredientMarket.ItemID = ingredientID;
            recipe.Ingredients = new List<IngredientPoco>() { ingredient };
            var itemIDList = new List<int>() { itemID, ingredientID };
            itemIDList.Sort();
            _marketDataGateway
                .GetMarketDataItems(default, default!)
                .ReturnsForAnyArgs(new List<MarketDataPoco>() { market, ingredientMarket });

            _recipeGateway
                .GetRecipesForItem(itemID)
                .Returns(new List<RecipePoco>() { recipe });
            _recipeGateway
                .GetRecipesForItem(ingredientID)
                .Returns(Array.Empty<RecipePoco>());
            _recipeGateway
                .GetRecipe(recipe.RecipeID)
                .Returns(recipe);

            var result = _calc!.CalculateCraftingCostForItem(WORLD_ID, itemID);

            _recipeGateway.Received().GetRecipesForItem(itemID);
            _recipeGateway.Received().GetRecipesForItem(ingredientID);
            _marketDataGateway.ReceivedWithAnyArgs().GetMarketDataItems(default, default!);
            Assert.That(result, Is.LessThan(int.MaxValue));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenNoRecipesExist_ThenReturnErrorCost()
        {
            int inexistentRecipeID = -200;
            _recipeGateway
                .GetRecipe(inexistentRecipeID)
                .Returns(_ => null!);

            var result = _calc!.CalculateCraftingCostForRecipe(WORLD_ID, inexistentRecipeID);

            _recipeGateway.Received().GetRecipe(inexistentRecipeID);
            _marketDataGateway.DidNotReceiveWithAnyArgs().GetMarketDataItems(default, default!);
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }
        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenARecipeExists_WhenNoMarketDataFound_ThenReturnErrorCost()
        {
            var recipe = _getNewRecipe();
            var recipeID = recipe.RecipeID;
            _recipeGateway.GetRecipe(recipeID).Returns(recipe);
            _marketDataGateway
                .GetMarketDataItems(WORLD_ID, default!)
                .ReturnsForAnyArgs(Array.Empty<MarketDataPoco>());

            var result = _calc!.CalculateCraftingCostForRecipe(WORLD_ID, recipeID);

            _recipeGateway.Received().GetRecipe(recipeID);
            _marketDataGateway.ReceivedWithAnyArgs().GetMarketDataItems(default, default!);
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculateCraftingCostForRecipe_WhenARecipeExists__ThenReturnCraftingCost()
        {
            var recipe = _getNewRecipe();
            var recipeID = recipe.RecipeID;
            _recipeGateway.GetRecipesForItem(recipeID).Returns(new List<RecipePoco>() { recipe });
            _recipeGateway.GetRecipe(recipeID).Returns(recipe);
            foreach (var ingredient in recipe.Ingredients)
                _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(_ => Array.Empty<RecipePoco>());

            var marketData = _getNewMarketData();
            var ingredientMarketDataList = new List<MarketDataPoco>();
            foreach (var ingredient in recipe.Ingredients)
            {
                var tempData = _getNewMarketData();
                tempData.ItemID = ingredient.ItemID;
                ingredientMarketDataList.Add(tempData);
            }
            var returnMarketData = new List<MarketDataPoco>() { marketData };
            returnMarketData.AddRange(ingredientMarketDataList);
            _marketDataGateway.GetMarketDataItems(WORLD_ID, Arg.Any<IEnumerable<int>>()).Returns(returnMarketData);

            var result = _calc!.CalculateCraftingCostForRecipe(WORLD_ID, recipeID);

            _recipeGateway
                .Received()
                .GetRecipe(recipeID);
            _recipeGateway
                .Received()
                .GetRecipesForItem(recipe.Ingredients[0].ItemID);
            _recipeGateway
                .Received()
                .GetRecipesForItem(recipe.Ingredients[1].ItemID);
            _recipeGateway
                .DidNotReceive()
                .GetRecipesForItem(recipe.TargetItemID);
            _marketDataGateway
                .Received()
                .GetMarketDataItems(WORLD_ID, Arg.Any<IEnumerable<int>>());
            Assert.That(result, Is.LessThan(100000000));
            Assert.That(result, Is.GreaterThan(3000));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenRecipeDoesNotExist_ThenReturnEmpty()
        {
            const int inexistentRecipeID = 1033;
            var recipePoco = _getNewRecipe();
            recipePoco.Ingredients = new List<IngredientPoco>() { recipePoco.Ingredients.First() };
            _recipeGateway.GetRecipe(inexistentRecipeID)
                          .Returns(_ => null!);

            var result = _calc!.BreakdownRecipe(inexistentRecipeID);

            _recipeGateway.Received(1).GetRecipe(inexistentRecipeID);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas1Ingredient_ThenReturn1()
        {
            const int existentRecipeID = 1033;
            var recipePoco = _getNewRecipe();
            recipePoco.Ingredients = new List<IngredientPoco>() { recipePoco.Ingredients.First() };
            var expectedTotalIngredientsCount = recipePoco.Ingredients
                .Select(x => x.Quantity)
                .Sum();
            _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

            var result = _calc!.BreakdownRecipe(existentRecipeID);

            var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
            _recipeGateway.Received(1).GetRecipe(existentRecipeID);
            Assert.Multiple(() =>
            {
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
                Assert.That(recipePoco.Ingredients, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas2Ingredients_ThenReturn2()
        {
            const int existentRecipeID = 1033;
            var recipePoco = _getNewRecipe();
            var expectedTotalIngredientsCount = recipePoco.Ingredients
                .Select(x => x.Quantity)
                .Sum();
            _recipeGateway.GetRecipe(existentRecipeID).Returns(recipePoco);

            var result = _calc!.BreakdownRecipe(existentRecipeID);

            var resultTotalIngredients = result.Select(x => x.Quantity).Sum();
            _recipeGateway.Received(1).GetRecipe(existentRecipeID);
            Assert.Multiple(() =>
            {
                Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
                Assert.That(resultTotalIngredients, Is.EqualTo(expectedTotalIngredientsCount));
                Assert.That(recipePoco.Ingredients, Has.Count.GreaterThanOrEqualTo(2));
            });
        }

        [Test]
        public void GivenACraftingCalculator_WhenBreakingDownARecipe_WhenItHas3Ingredients_ThenReturn3()
        {
            var recipe = _getNewRecipe();
            var recipeID = recipe.RecipeID;

            recipe.Ingredients.Add(new pocos.IngredientPoco(753, 5, recipeID));
            _recipeGateway.GetRecipe(recipeID).Returns(recipe);
            _mockIngredientsToReturnEmpty(recipe.Ingredients);
            var expectedIngredientsSum = recipe.Ingredients.Select(x => x.Quantity).Sum();
            var expectedIngredientsCount = recipe.Ingredients.Count;

            var result = _calc!.BreakdownRecipe(recipeID);

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
            var firstRecipeID = firstRecipe.RecipeID;
            Assume.That(firstRecipe.Ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            var subItem1 = _getNewMarketData();
            subItem1.ItemID = subItem1ID;
            var subItem2 = _getNewMarketData();
            subItem2.ItemID = subItem2ID;
            var secondRecipe = _getSecondRecipe(subItem1, subItem2);
            Assume.That(secondRecipe.Ingredients.Count(), Is.GreaterThanOrEqualTo(2));
            var expectedIngredients = _getExpectedIngredientsFromRecipes(firstRecipe, secondRecipe);
            var expectedIngredientsSum = expectedIngredients.Select(x => x.Quantity).Sum();
            var expectedIngredientsCount = expectedIngredients.Count;
            Assume.That(expectedIngredientsCount, Is.GreaterThanOrEqualTo(3));
            _mockRecipeGatewayForTwoRecipes(firstRecipe, secondRecipe);

            var result = _calc!.BreakdownRecipe(firstRecipeID);

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
            Assume.That(secondRecipe.Ingredients.Count, Is.GreaterThan(1));
            Assume.That(firstRecipe.Ingredients.Count, Is.GreaterThan(1));
            _recipeGateway.GetRecipe(firstRecipe.RecipeID).Returns(firstRecipe);
            _recipeGateway.GetRecipe(secondRecipe.RecipeID).Returns(secondRecipe);
            _recipeGateway.GetRecipesForItem(firstItemID).Returns(Array.Empty<RecipePoco>());
            _recipeGateway.GetRecipesForItem(secondItemID).Returns(new List<RecipePoco>() { secondRecipe });
            _mockIngredientsToReturnEmpty(secondRecipe.Ingredients);
        }

        private void _mockIngredientsToReturnEmpty(IEnumerable<IngredientPoco> ingredients)
        {
            foreach (var ingredient in ingredients)
                _recipeGateway.GetRecipesForItem(ingredient.ItemID).Returns(Array.Empty<RecipePoco>());
        }

        private List<IngredientPoco> _getExpectedIngredientsFromRecipes(RecipePoco firstRecipe, RecipePoco secondRecipe)
        {
            var secondRecipeIngredients = secondRecipe.Ingredients;
            List<IngredientPoco> expectedIngredients = new List<IngredientPoco>(secondRecipeIngredients);
            Assume.That(expectedIngredients, Is.Not.Empty);

            IngredientPoco firstRecipeIngredient = firstRecipe.Ingredients.FirstOrDefault()!;
            Assume.That(firstRecipeIngredient, Is.Not.Null);

            expectedIngredients.Add(new IngredientPoco(firstRecipeIngredient));
            Assume.That(expectedIngredients.Count, Is.GreaterThan(secondRecipeIngredients.Count));
            return expectedIngredients;
        }

        private RecipePoco _getSecondRecipe(MarketDataPoco subItem1, MarketDataPoco subItem2)
        {
            RecipePoco recipe = _getNewRecipe();
            recipe.RecipeID = secondRecipeID;
            recipe.TargetItemID = secondItemID;

            var ingredient1 = new IngredientPoco(subItem1.ItemID, 6, secondRecipeID);
            var ingredient2 = new IngredientPoco(subItem2.ItemID, 7, secondRecipeID);
            recipe.Ingredients = new List<IngredientPoco>() { ingredient1, ingredient2 };

            return recipe;
        }

        private MarketDataPoco _getNewMarketData()
        {
            return new MarketDataPoco(1, WORLD_ID, 1, "Iron Sword", "testRealm", 300, 200, 400, 600, 400, 800);
        }

        private RecipePoco _getNewRecipe()
        {
            return new RecipePoco(
                true, true, targetItemID, recipeID, 254, 1, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, firstItemID, secondItemID, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
