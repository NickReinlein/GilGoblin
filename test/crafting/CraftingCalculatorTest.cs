using GilGoblin.pocos;
using GilGoblin.web;
using GilGoblin.crafting;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GilGoblin.Test.crafting
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private ICraftingCalculator _calc = Substitute.For<ICraftingCalculator>();
        private ILogger _log = Substitute.For<ILogger>();
        private int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataPoco _poco = new MarketDataPoco(
            1,
            1,
            1,
            "test",
            "testRealm",
            300,
            200,
            400,
            600,
            400,
            800
        );
        private MarketDataPoco[] _pocos = Array.Empty<MarketDataPoco>();

        private const int WORLD_ID = 34; // Brynnhildr

        [SetUp]
        public void setUp()
        {
            _pocos = new MarketDataPoco[] { _poco };
        }

        [TearDown]
        public void tearDown()
        {
            _calc.ClearReceivedCalls();
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesNotExist_ReturnErrorCost()
        {
            int inexistentItemID = -200;
            _calc.CalculateCraftingCost(WORLD_ID, inexistentItemID).Returns(ERROR_COST);

            var result = _calc.CalculateCraftingCost(WORLD_ID, inexistentItemID);

            _calc.Received(1).CalculateCraftingCost(WORLD_ID, inexistentItemID);
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesExist_ReturnCraftingCost()
        {
            const int existentRecipeID = 1033;
            const int craftingCost = 250;
            _calc.CalculateCraftingCost(WORLD_ID, existentRecipeID).Returns(craftingCost);

            var result = _calc.CalculateCraftingCost(WORLD_ID, existentRecipeID);

            _calc.Received(1).CalculateCraftingCost(WORLD_ID, existentRecipeID);
            // throws
            Assert.That(result, Is.EqualTo(craftingCost));
        }
    }
}
