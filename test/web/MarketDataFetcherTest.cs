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
        private MarketDataWebPoco? poco;

        [SetUp]
        public void setUp()
        {
            poco = new MarketDataWebPoco();
            _marketData = Substitute.For<IMarketDataWeb>();
            //_marketData.FetchMarketData(Arg.Any<int>(), Arg.Any<int>()).Returns(poco);
        }
        [TearDown]
        public void tearDown()
        {
            _marketData.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketFetcher_WhenRequestingASingleItem_ThenASingleItemIsReturned()
        {            
            _marketData.FetchMarketData(1, 1).Returns(poco);
            var result = _marketData.FetchMarketData(1, 1);

            _marketData.Received().FetchMarketData(1, 1);
            Assert.That(result, Is.TypeOf<MarketDataWebPoco>());
            // additional asserts for sent            
        }
        [Test]
        public void GivenAMarketFetcher_WhenRequestingInexistantItem_ThenNullIsReturned()
        {
            _marketData.FetchMarketData(Arg.Any<int>(), Arg.Any<int>()).Returns(_ => null);
            var result = _marketData.FetchMarketData(1, 1);

            _marketData.Received().FetchMarketData(1, 1);
            Assert.IsNull(result);
            // additional asserts for sent            
        }
    }
}