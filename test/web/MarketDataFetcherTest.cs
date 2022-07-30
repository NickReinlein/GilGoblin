using GilGoblin.Pocos;
using GilGoblin.web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class MarketDataFetcherTest
    {
        private IMarketDataWeb _marketData = Substitute.For<IMarketDataWeb>();
        private MarketDataFetcher _fetcher = new MarketDataFetcher();
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataWebPoco _poco = new MarketDataWebPoco(1,1,1,"test","testRealm", 300,200,400, 600,400,800);
        private MarketDataWebPoco[] _pocos = Array.Empty<MarketDataWebPoco>();

        [SetUp]
        public void setUp()
        {        
            _pocos = new MarketDataWebPoco[] { _poco };
        }
        [TearDown]
        public void tearDown()
        {
            _marketData.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketFetcher_WhenFetchingItemsSucessfully_ThenItemsAreReturned()
        {
            mockFetch<IEnumerable<MarketDataWebPoco>>(_pocos);

            var result = _marketData.FetchMarketDataItems(1, itemIDs);

            _marketData.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));;
        }


        [Test]
        public void GivenAMarketFetcher_WhenFetchingItemsWithWeHaveDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
        {
            var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
            var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();

            var result = _fetcher.FetchMarketDataItems(1, itemIdListWithDupes);

            var resultIDs = result.Select(x => x.itemID);
            Assert.That(resultIDs.Count(), Is.EqualTo(itemIdListWithoutDupes.Count()));
        }        
        
        [Test]
        public void GivenAMarketFetcher_WhenRequestingInexistantItems_ThenNothingIsReturned()
        {
            mockFetch<IEnumerable<MarketDataWebPoco>>(Array.Empty<MarketDataWebPoco>());

            var result = _marketData.FetchMarketDataItems(1, itemIDs);

            _marketData.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GivenAMarketFetcher_WhenRequestingSomeItemsSuccessfulSomeFail_ThenSuccessulAreReturned()
        {
            var _poco1 = new MarketDataWebPoco(_poco);
            _poco1.itemID = 444;
            var _poco2 = new MarketDataWebPoco(_poco);
            _poco2.itemID = 777;
            var existingItems = new MarketDataWebPoco[] { _poco1,_poco2 };
            mockFetch<IEnumerable<MarketDataWebPoco>>(existingItems);
            Assume.That(itemIDs.Count(), Is.GreaterThan(existingItems.Count()));

            var result = _marketData.FetchMarketDataItems(1, itemIDs);

            _marketData.Received().FetchMarketDataItems(1,itemIDs);
            Assert.That(result, Is.EquivalentTo(existingItems));
        }

        public void mockFetch<T>(IEnumerable<MarketDataWebPoco> pocosReturned){
            _marketData.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<IEnumerable<int>>()).Returns(pocosReturned);
        }
    }
}