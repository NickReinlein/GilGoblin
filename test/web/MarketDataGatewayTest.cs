using GilGoblin.pocos;
using GilGoblin.web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class MarketDataGatewayTest
    {
        private IMarketDataGateway _mock = Substitute.For<IMarketDataGateway>();
        private MarketDataGateway _gateway = new MarketDataGateway();
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
            _mock.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSucessfull_ThenItemsAreReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(_pocos);

            var result = _mock.FetchMarketDataItems(1, itemIDs);

            _mock.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));;
        }


        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();

            var result = _gateway.FetchMarketDataItems(1, itemIdListWithDupes);

            var resultIDs = result.Select(x => x.itemID);
            Assert.That(resultIDs.Count(), Is.EqualTo(itemIdListWithoutDupes.Count()));
        }        
        
        [Test]
        public void GivenAMarketDataGateway_WhenFetchingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
        {
            mockFetch<IEnumerable<MarketDataPoco>>(Array.Empty<MarketDataPoco>());

            var result = _mock.FetchMarketDataItems(1, itemIDs);

            _mock.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
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

            var result = _mock.FetchMarketDataItems(1, itemIDs);

            _mock.Received().FetchMarketDataItems(1,itemIDs);
            Assert.That(result, Is.EquivalentTo(existingItems));
        }

        private void mockFetch<T>(IEnumerable<MarketDataPoco> pocosReturned){
            _mock.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<IEnumerable<int>>()).Returns(pocosReturned);
        }
    }
}