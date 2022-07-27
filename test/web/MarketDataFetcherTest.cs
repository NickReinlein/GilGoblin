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

        [SetUp]
        public void setUp()
        {
            _marketData = Substitute.For<IMarketDataWeb>();
            _fetcher = new MarketDataFetcher();
        }
        [TearDown]
        public void tearDown()
        {
            _marketData.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketFetcher_WhenFetchingItemsSucessfully_ThenItemsAreReturned()
        {
            _pocos = new MarketDataWebPoco[] { new MarketDataWebPoco() };
            _marketData.FetchMarketDataItems(1, itemIDs).Returns(_pocos);
            var result = _fetcher.FetchMarketDataItems(1, itemIDs);

            const callWasReceived = _marketData.Received().FetchMarketDataItems(1, itemIDs);
           // Assert.That(callWasReceived, Is.True);
            Assert.That(result, Is.TypeOf<MarketDataWebPoco[]>());
            // additional asserts for sent            
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
    }
}