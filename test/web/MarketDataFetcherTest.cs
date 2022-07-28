using GilGoblin.Pocos;
using GilGoblin.web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class MarketDataFetcherTest
    {
        private IMarketDataWeb _marketData;
        private MarketDataFetcher _fetcher;
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataWebPoco[] _pocos;
        private MarketDataWebPoco _poco;

        [SetUp]
        public void setUp()
        {
            _marketData = Substitute.For<IMarketDataWeb>();
            _fetcher = new MarketDataFetcher();
            _poco = new MarketDataWebPoco(1,1,1,"test","testRealm", 300,200,400, 600,400,800);
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
            mockFetch<MarketDataWebPoco[]>(_pocos);

            var result = _marketData.FetchMarketDataItems(1, itemIDs);

            _marketData.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));;
        }
        
        [Test]
        public void GivenAMarketFetcher_WhenRequestingInexistantItems_ThenNothingIsReturned()
        {
            _pocos = new MarketDataWebPoco[]{};
            mockFetch<MarketDataWebPoco[]>(_pocos);

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
            mockFetch<MarketDataWebPoco[]>(existingItems);

            var result = _marketData.FetchMarketDataItems(1, itemIDs);

            _marketData.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            Assert.That(result.Count(), Is.GreaterThan(0));
            Assert.That(itemIDs.Count(), Is.GreaterThan(0));
            Assert.That(_poco1, Is.SubsetOf(result));
        }

        public void mockFetch<T>(IEnumerable<MarketDataWebPoco> pocosReturned){
            _marketData.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>()).Returns(pocosReturned);
        }
    }
}