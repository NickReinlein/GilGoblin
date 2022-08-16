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
        private CraftingCalculator _calc = new CraftingCalculator();
        private IRecipeGateway _recipeGateway = Substitute.For<IRecipeGateway>();
        private IMarketDataGateway _marketDataGateway = Substitute.For<IMarketDataGateway>();
        private ILogger _log = Substitute.For<ILogger>();
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataPoco? _poco;
        private MarketDataPoco[]? _pocos;

        private int ERROR_COST = CraftingCalculator.ERROR_DEFAULT_COST;
        private const int WORLD_ID = 34; // Brynnhildr

        [SetUp]
        public void setUp()
        {
            _poco = _getGoodPoco();
            _pocos = new MarketDataPoco[] { _poco };
            _calc = new CraftingCalculator(_recipeGateway, _marketDataGateway);
        }

        [TearDown]
        public void tearDown()
        {
            _recipeGateway.ClearReceivedCalls();
            _marketDataGateway.ClearReceivedCalls();
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesNotExist_ReturnErrorCost()
        {
            int inexistentItemID = -200;
            _marketDataGateway
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>())
                .ReturnsForAnyArgs(Array.Empty<MarketDataPoco>());

            var result = _calc.CalculateCraftingCost(WORLD_ID, inexistentItemID);

            _marketDataGateway
                .ReceivedWithAnyArgs(1)
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>());
            Assert.That(result, Is.EqualTo(ERROR_COST));
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesExist_ReturnCraftingCost()
        {
            const int existentRecipeID = 1033;
            MarketDataPoco pocoReturned = _getGoodPoco();
            _marketDataGateway
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>())
                .ReturnsForAnyArgs(new List<MarketDataPoco>() { pocoReturned });

            var result = _calc.CalculateCraftingCost(WORLD_ID, existentRecipeID);

            _marketDataGateway
                .ReceivedWithAnyArgs(1)
                .GetMarketDataItems(default, Arg.Any<IEnumerable<int>>());
            Assert.That(result, Is.EqualTo(pocoReturned.averageSale));
        }

        private static MarketDataPoco _getGoodPoco()
        {
            return new MarketDataPoco(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
        }
    }
}
