using GilGoblin.Pocos;
using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.Web
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private readonly IMarketDataGateway _gateway = Substitute.For<IMarketDataGateway>();
        private readonly int[] _itemIDs = { 1, 2, 3, 4, 5, 6 };
        private readonly MarketDataPoco _poco = new(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
        private MarketDataPoco[] _pocos = Array.Empty<MarketDataPoco>();

        [SetUp]
        public void SetUp()
        {
            _pocos = new MarketDataPoco[] { _poco };
        }
        [TearDown]
        public void TearDown()
        {
            _gateway.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSucessfull_ThenItemsAreReturned()
        {
            MockFetch<IEnumerable<MarketDataPoco>>(_pocos);

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos)); ;
        }


        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
            var itemsWithoutDupes = ConvertItemIDsToPocos(itemIdListWithoutDupes);
            _gateway.FetchMarketDataItems(Arg.Any<int>(), itemIdListWithDupes).Returns(itemsWithoutDupes);

            var result = _gateway.FetchMarketDataItems(1, itemIdListWithDupes);

            Assert.That(result, Is.EqualTo(itemsWithoutDupes));
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
        {
            MockFetch<IEnumerable<MarketDataPoco>>(Array.Empty<MarketDataPoco>());

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
        {
<<<<<<< HEAD
            var _poco1 = new MarketDataPoco(_poco);
            _poco1.ItemID = 444;
            var _poco2 = new MarketDataPoco(_poco);
            _poco2.ItemID = 777;
            var existingItems = new MarketDataPoco[] { _poco1,_poco2 };
            mockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(itemIDs.Count(), Is.GreaterThan(existingItems.Count()));

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(1,itemIDs);
=======
            var poco1 = new MarketDataPoco(_poco)
            {
                ItemID = 444
            };
            var poco2 = new MarketDataPoco(_poco)
            {
                ItemID = 777
            };
            var existingItems = new MarketDataPoco[] { poco1, poco2 };
            MockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(_itemIDs.Length, Is.GreaterThan(existingItems.Length));

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(1, _itemIDs);
>>>>>>> add-crafting-calculator-part-3
            Assert.That(result, Is.EquivalentTo(existingItems));
        }

        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSucessfull_ThenItemsAreReturned()
        {
            MockFetch<IEnumerable<MarketDataPoco>>(_pocos);

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos)); ;
        }


        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
            var itemsWithoutDupes = ConvertItemIDsToPocos(itemIdListWithoutDupes);
            _gateway.GetMarketDataItems(Arg.Any<int>(), itemIdListWithDupes).Returns(itemsWithoutDupes);

            var result = _gateway.GetMarketDataItems(1, itemIdListWithDupes);

            Assert.That(result, Is.EqualTo(itemsWithoutDupes));
        }

        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
        {
            MockFetch<IEnumerable<MarketDataPoco>>(Array.Empty<MarketDataPoco>());

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
        {
<<<<<<< HEAD
            var _poco1 = new MarketDataPoco(_poco);
            _poco1.ItemID = 444;
            var _poco2 = new MarketDataPoco(_poco);
            _poco2.ItemID = 777;
            var existingItems = new MarketDataPoco[] { _poco1,_poco2 };
            mockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(itemIDs.Count(), Is.GreaterThan(existingItems.Count()));

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(1,itemIDs);
=======
            var poco1 = new MarketDataPoco(_poco)
            {
                ItemID = 444
            };
            var poco2 = new MarketDataPoco(_poco)
            {
                ItemID = 777
            };
            var existingItems = new MarketDataPoco[] { poco1, poco2 };
            MockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(_itemIDs.Length, Is.GreaterThan(existingItems.Length));

            var result = _gateway.FetchMarketDataItems(1, _itemIDs);

            _gateway.Received().FetchMarketDataItems(1, _itemIDs);
>>>>>>> add-crafting-calculator-part-3
            Assert.That(result, Is.EquivalentTo(existingItems));
        }

        private void MockFetch<T>(IEnumerable<MarketDataPoco> pocosReturned)
        {
            _gateway.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<IEnumerable<int>>()).Returns(pocosReturned);
        }
        private static IEnumerable<MarketDataPoco> ConvertItemIDsToPocos(IEnumerable<int> itemIDs)
        {
            return itemIDs.Select(x => new MarketDataPoco(x, 1, 1, "fake", "fakeRealm", 3, 2, 4, 6, 4, 8));
        }
    }
}