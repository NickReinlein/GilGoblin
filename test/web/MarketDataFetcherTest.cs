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
            Assert.That(result, Is.TypeOf<MarketDataWebPoco[]>());
            Assert.That(result, Is.EquivalentTo(_pocos));
            Assert.That(result.Count(),Is.EqualTo(_pocos.Count()));
        }
        
        [Test]
        public void GivenAMarketFetcher_WhenRequestingInexistantItem_ThenNullIsReturned()
        {
            // _marketData.FetchMarketData(Arg.Any<int>(), Arg.Any<int>()).Returns(_ => null);
            // var result = _marketData.FetchMarketData(1, 1);

            // _marketData.Received().FetchMarketData(1, 1);
            // Assert.IsNull(result);
            // // additional asserts for sent            
        }

        public void mockFetch<T>(IEnumerable<MarketDataWebPoco> pocosReturned){
            _marketData.FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>()).Returns(pocosReturned);
        }
    }
}