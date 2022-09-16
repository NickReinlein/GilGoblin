using GilGoblin.Pocos;
using GilGoblin.Web;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Web
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
        public void GivenARecipeGateway_WhenGettingAllRecipesForItem_WhenRecipeDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentRecipeID = -1;
            _gateway.GetRecipesForItem(inexistentRecipeID).ReturnsNull();

            var result = _gateway.GetRecipesForItem(inexistentRecipeID);

            _gateway.Received(1).GetRecipesForItem(inexistentRecipeID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenARecipeGateway_WhenGettingAllRecipesForItem_When1RecipeExists_Then1RecipeIsReturned()
        {
            const int existentRecipeID = 1033;
            var existentRecipeForItem = new List<RecipePoco>() { _poco };
            _gateway.GetRecipesForItem(existentRecipeID).Returns(existentRecipeForItem);

            var result = _gateway.GetRecipesForItem(existentRecipeID);

            _gateway.Received(1).GetRecipesForItem(existentRecipeID);
            Assert.That(result, Is.EqualTo(existentRecipeForItem));
        }

        [Test]
        public void GivenARecipeGateway_WhenGettingAllRecipesForItem_When2RecipesExist_Then2RecipeAreReturned()
        {
            const int existentRecipeID = 1033;
            var poco2 = new RecipePoco(_poco)
            {
                TargetItemID = 333,
                RecipeID = 2900
            };
            var existentRecipeForItem = new List<RecipePoco>() { _poco, poco2 };
            _gateway.GetRecipesForItem(existentRecipeID).Returns(existentRecipeForItem);

            var result = _gateway.GetRecipesForItem(existentRecipeID);

            _gateway.Received(1).GetRecipesForItem(existentRecipeID);
            Assert.That(result, Is.EqualTo(existentRecipeForItem));
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