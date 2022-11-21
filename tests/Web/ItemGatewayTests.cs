using GilGoblin.Pocos;
using GilGoblin.Web;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Web
{
    [TestFixture]
    public class ItemGatewayTest
    {
        private readonly IItemGateway _gateway = Substitute.For<IItemGateway>();
        private ItemInfoPoco _poco = new();

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
        public void GivenAnItemGateway_WhenGettingAnItem_WhenItemDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentItemID = -1;
            _gateway.GetItem(inexistentItemID).ReturnsNull();

            var result = _gateway.GetItem(inexistentItemID);

            _gateway.Received(1).GetItem(inexistentItemID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenAnItemGateway_WhenGettingAnItem_WhenItemDoesExist_ThenTheItemIsReturned()
        {
            const int existentItemID = 1033;
            _gateway.GetItem(existentItemID).Returns(_poco);

            var result = _gateway.GetItem(existentItemID);

            _gateway.Received(1).GetItem(existentItemID);
            Assert.That(result, Is.EqualTo(_poco));
        }

        [Test]
        public void GivenAnItemGateway_WhenGettingAllItemsForItem_WhenItemDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentItemID = -1;
            _gateway.GetItem(inexistentItemID).ReturnsNull();

            var result = _gateway.GetItem(inexistentItemID);

            _gateway.Received(1).GetItem(inexistentItemID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenAnItemGateway_WhenGettingAllItemsForItem_When1ItemExists_Then1ItemIsReturned()
        {
            const int existentItemID = 1033;
            var existentItemIds = new List<int>() { existentItemID };
            var existentItems = new List<ItemInfoPoco>() { _poco };
            _gateway.GetItems(existentItemIds).Returns(existentItems);

            var result = _gateway.GetItems(existentItemIds);

            _gateway.Received(1).GetItems(existentItemIds);
            Assert.That(result, Is.EqualTo(existentItems));
        }

        [Test]
        public void GivenAnItemGateway_WhenGettingAllItemsForItem_When2ItemsExist_Then2ItemAreReturned()
        {
            const int existentItemID = 1033;
            var poco2 = new ItemInfoPoco(_poco)
            {
                TargetItemID = 333,
                ItemID = 2900
            };
            var existentItemForItem = new List<ItemInfoPoco>() { _poco, poco2 };
            _gateway.GetItems(existentItemID).Returns(existentItemForItem);

            var result = _gateway.GetItems(existentItemID);

            _gateway.Received(1).GetItems(existentItemID);
            Assert.That(result, Is.EqualTo(existentItemForItem));
        }

        [Test]
        public void GivenAnItemGateway_WhenFetchingAnItem_WhenItemDoesNotExist_ThenNullIsReturned()
        {
            const int inexistentItemID = -1;
            _gateway.FetchItem(inexistentItemID).ReturnsNull();

            var result = _gateway.FetchItem(inexistentItemID);

            _gateway.Received(1).FetchItem(inexistentItemID);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenAnItemGateway_WhenFetchingAnItem_WhenItemDoesExist_ThenTheItemIsReturned()
        {
            const int existentItemID = 1033;
            _gateway.FetchItem(existentItemID).Returns(_poco);

            var result = _gateway.FetchItem(existentItemID);

            _gateway.Received(1).FetchItem(existentItemID);
            Assert.That(result, Is.EqualTo(_poco));
        }

        private void SetupPoco()
        {
            _poco = new ItemInfoPoco(true, true, 50, 1, 323, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4, 0, 0, 0, 0, 0, 0);
        }
    }
}