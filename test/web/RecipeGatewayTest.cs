using GilGoblin.pocos;
using GilGoblin.web;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Test.web
{
    [TestFixture]
    public class RecipeGatewayTest
    {
        private IRecipeGateway _mock = Substitute.For<IRecipeGateway>();
        private RecipeGateway _gateway = new RecipeGateway();
        private RecipeFullPoco _poco = new RecipeFullPoco();
    
        [SetUp]
        public void setUp()
        {
            setupPoco();
        }

        [TearDown]
        public void tearDown()
        {
            _mock.ClearReceivedCalls();
        }

        [Test]
        public void GivenARecipeGateway_WhenRecipeIDDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentRecipeID = -1;
            _mock.GetRecipe(inexistentRecipeID).ReturnsNull();
            
            var result = _mock.GetRecipe(inexistentRecipeID);

            _mock.Received(1).GetRecipe(inexistentRecipeID);
            Assert.That(result, Is.Null);
        }

        private void setupPoco()
        {
            _poco = new RecipeFullPoco(true, true, 50, 1, 323, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4, 0, 0, 0, 0, 0, 0);
        }
    }
}