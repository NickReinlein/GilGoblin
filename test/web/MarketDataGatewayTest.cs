using GilGoblin.pocos;
using GilGoblin.web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private IMarketDataGateway _gateway = Substitute.For<IMarketDataGateway>();
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataPoco _poco = new MarketDataPoco(1,1,1,"test","testRealm", 300,200,400, 600,400,800);
        private MarketDataPoco[] _pocos = Array.Empty<MarketDataPoco>();

        [SetUp]
        public void setUp()
        {        
            _pocos = new MarketDataPoco[] { _poco };
        }
        [TearDown]
        public void tearDown()
        {
            _gateway.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSucessfull_ThenItemsAreReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(_pocos);

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));;
        }


        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
            var itemsWithoutDupes = convertItemIDsToPocos(itemIdListWithoutDupes);
            _gateway.FetchMarketDataItems(Arg.Any<int>(),itemIdListWithDupes).Returns(itemsWithoutDupes);

            var result = _gateway.FetchMarketDataItems(1, itemIdListWithDupes);

            Assert.That(result, Is.EqualTo(itemsWithoutDupes));
        }        
        
        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(Array.Empty<MarketDataPoco>());

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
        {
            var _poco1 = new MarketDataPoco(_poco);
            _poco1.itemID = 444;
            var _poco2 = new MarketDataPoco(_poco);
            _poco2.itemID = 777;
            var existingItems = new MarketDataPoco[] { _poco1,_poco2 };
            mockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(itemIDs.Count(), Is.GreaterThan(existingItems.Count()));

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(1,itemIDs);
            Assert.That(result, Is.EquivalentTo(existingItems));
        }

        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSucessfull_ThenItemsAreReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(_pocos);

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));;
        }


        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
            var itemsWithoutDupes = convertItemIDsToPocos(itemIdListWithoutDupes);
            _gateway.GetMarketDataItems(Arg.Any<int>(),itemIdListWithDupes).Returns(itemsWithoutDupes);

            var result = _gateway.GetMarketDataItems(1, itemIdListWithDupes);

            Assert.That(result, Is.EqualTo(itemsWithoutDupes));
        }        
        
        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(Array.Empty<MarketDataPoco>());

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenAMarketDataGateway_WhenGettingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
        {
            var _poco1 = new MarketDataPoco(_poco);
            _poco1.itemID = 444;
            var _poco2 = new MarketDataPoco(_poco);
            _poco2.itemID = 777;
            var existingItems = new MarketDataPoco[] { _poco1,_poco2 };
            mockFetch<IEnumerable<MarketDataPoco>>(existingItems);
            Assume.That(itemIDs.Count(), Is.GreaterThan(existingItems.Count()));

            var result = _gateway.FetchMarketDataItems(1, itemIDs);

            _gateway.Received().FetchMarketDataItems(1,itemIDs);
            Assert.That(result, Is.EquivalentTo(existingItems));
        }        

        private void mockFetch<T>(IEnumerable<MarketDataPoco> pocosReturned){
            _gateway.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<IEnumerable<int>>()).Returns(pocosReturned);
        }
        private IEnumerable<MarketDataPoco> convertItemIDsToPocos(IEnumerable<int> itemIDs){
            return itemIDs.Select(x => new MarketDataPoco(x,1,1,"fake","fakeRealm", 3,2,4,6,4,8));
        }
    }
}