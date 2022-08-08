using GilGoblin.pocos;
using GilGoblin.web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class RecipeGatewayTest
    {
        private IRecipeGateway _recipeGetter = Substitute.For<IRecipeGateway>();
        private RecipeGateway _fetcher;// = new MarketDataFetcher();
        


        [SetUp]
        public void setUp()
        {        
            // _pocos = new MarketDataPoco[] { _poco };
            _fetcher = new RecipeGateway();
        }
        [TearDown]
        public void tearDown()
        {
            // _marketData.ClearReceivedCalls();
        }

        [Test]
        public void GivenAMarketFetcher_WhenFetchingItemsSucessfully_ThenItemsAreReturned()
        {}
    }
}