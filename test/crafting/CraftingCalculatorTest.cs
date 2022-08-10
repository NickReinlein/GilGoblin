using GilGoblin.pocos;
using GilGoblin.web;
using GilGoblin.crafting;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Test.crafting
{
    [TestFixture]
    public class CraftingCalculatorTest
    {
        private ICraftingCalculator _calc = Substitute.For<ICraftingCalculator>();
        private int[] itemIDs = { 1, 2, 3, 4, 5, 6 };
        private MarketDataPoco _poco = new MarketDataPoco(1,1,1,"test","testRealm", 300,200,400, 600,400,800);
        private MarketDataPoco[] _pocos = Array.Empty<MarketDataPoco>();

        private const int WORLD_ID = 34;      // Brynnhildr

        [SetUp]
        public void setUp()
        {        
            _pocos = new MarketDataPoco[] { _poco };
        }
        [TearDown]
        public void tearDown()
        {
            _calc.ClearReceivedCalls();
        }

        [Test]
        public void GivenACraftingCalculator_WhenCalculatingCost_WhenItemDoesNotExist_ReturnNull()
        {
            // int inexistentItemID = -200;        
            // _calc.calculateCraftingCost(inexistentItemID,WORLD_ID).Returns(null);

            // var result = _calc.calculateCraftingCost(1, itemIDs);

            // _calc.Received().FetchMarketDataItems(Arg.Any<int>(), Arg.Any<int[]>());
            // Assert.That(result, Is.EquivalentTo(_pocos));;
        }
        
        //             const int inexistentRecipeID = -1;
        //     _gateway.FetchRecipe(inexistentRecipeID).ReturnsNull();
            
        //     var result = _gateway.FetchRecipe(inexistentRecipeID);

        //     _gateway.Received(1).FetchRecipe(inexistentRecipeID);
        //     Assert.That(result, Is.Null);
        // }

        // [Test]
        // public void GivenARecipeGateway_WhenFetchingARecipe_WhenRecipeDoesExist_ThenTheRecipeIsReturned()
        // {
        //     const int existentRecipeID = 1033;
        //     _gateway.FetchRecipe(existentRecipeID).Returns(_poco);
            
        //     var result = _gateway.FetchRecipe(existentRecipeID);

        //     _gateway.Received(1).FetchRecipe(existentRecipeID);
        //     Assert.That(result, Is.EqualTo(_poco));
        // }
    }
}