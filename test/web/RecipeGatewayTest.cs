using GilGoblin.Pocos;
using GilGoblin.Web;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Test.Web
{
    [TestFixture]
    public class RecipeGatewayTest
    {
        private readonly IRecipeGateway _gateway = Substitute.For<IRecipeGateway>();
        private RecipePoco _poco = new();

        [SetUp]
        public void SetUp()
        {
            SetupPoco();
        }

        [TearDown]
        public void TearDown()
        {
            _gateway.ClearReceivedCalls();
        }

        [Test]
        public void GivenARecipeGateway_WhenGettingARecipe_WhenRecipeDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentRecipeID = -1;
            _gateway.GetRecipe(inexistentRecipeID).ReturnsNull();

            var result = _gateway.GetRecipe(inexistentRecipeID);

            _gateway.Received(1).GetRecipe(inexistentRecipeID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenARecipeGateway_WhenGettingARecipe_WhenRecipeDoesExist_ThenTheRecipeIsReturned()
        {
            const int existentRecipeID = 1033;
            _gateway.GetRecipe(existentRecipeID).Returns(_poco);

            var result = _gateway.GetRecipe(existentRecipeID);

            _gateway.Received(1).GetRecipe(existentRecipeID);
            Assert.That(result, Is.EqualTo(_poco));
        }
        [Test]
        public void GivenARecipeGateway_WhenFetchingARecipe_WhenRecipeDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentRecipeID = -1;
            _gateway.FetchRecipe(inexistentRecipeID).ReturnsNull();

            var result = _gateway.FetchRecipe(inexistentRecipeID);

            _gateway.Received(1).FetchRecipe(inexistentRecipeID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenARecipeGateway_WhenFetchingARecipe_WhenRecipeDoesExist_ThenTheRecipeIsReturned()
        {
            const int existentRecipeID = 1033;
            _gateway.FetchRecipe(existentRecipeID).Returns(_poco);

            var result = _gateway.FetchRecipe(existentRecipeID);

            _gateway.Received(1).FetchRecipe(existentRecipeID);
            Assert.That(result, Is.EqualTo(_poco));
        }

        private void SetupPoco()
        {
            _poco = new RecipePoco(true, true, 50, 1, 323, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4, 0, 0, 0, 0, 0, 0);
        }
    }
}