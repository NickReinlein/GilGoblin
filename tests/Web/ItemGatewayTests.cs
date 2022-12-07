using GilGoblin.Pocos;
using GilGoblin.Web;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Web;

public class ItemGatewayTests
{
    private readonly IItemGateway _gateway = Substitute.For<IItemGateway>();
    private ItemInfoPoco _poco = new();
    const int existentItemID = 1033;
    const int inexistentItemID = -1;

    [SetUp]
    public void SetUp()
    {
        SetupBasicPoco();
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
        _gateway.GetItem(existentItemID).Returns(_poco);

        var result = _gateway.GetItem(existentItemID);

        _gateway.Received(1).GetItem(existentItemID);
        Assert.That(result, Is.EqualTo(_poco));
    }

    [Test]
    public void GivenAnItemGateway_WhenGettingAllItemsForItem_WhenItemDoesNotExist_ThenNullIsReturned()
    {
        _gateway.GetItem(inexistentItemID).ReturnsNull();

        var result = _gateway.GetItem(inexistentItemID);

        _gateway.Received(1).GetItem(inexistentItemID);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAnItemGateway_WhenGettingAllItemsForItem_When1ItemExists_Then1ItemIsReturned()
    {
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
        var poco2 = new ItemInfoPoco(_poco) { ID = 1055 };
        var existentItems = new List<ItemInfoPoco>() { _poco, poco2 };
        var existentItemIds = existentItems.Select((item) => item.ID).ToList();
        _gateway.GetItems(existentItemIds).Returns(existentItems);

        var result = _gateway.GetItems(existentItemIds);

        _gateway.Received(1).GetItems(existentItemIds);
        Assert.That(result, Is.EqualTo(existentItems));
    }

    [Test]
    public void GivenAnItemGateway_WhenFetchingAnItem_WhenItemDoesNotExist_ThenNullIsReturned()
    {
        _gateway.GetItem(inexistentItemID).ReturnsNull();

        var result = _gateway.GetItem(inexistentItemID);

        _gateway.Received(1).GetItem(inexistentItemID);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAnItemGateway_WhenFetchingAnItem_WhenItemDoesExist_ThenTheItemIsReturned()
    {
        _gateway.GetItem(existentItemID).Returns(_poco);

        var result = _gateway.GetItem(existentItemID);

        _gateway.Received(1).GetItem(existentItemID);
        Assert.That(result, Is.EqualTo(_poco));
    }

    private void SetupBasicPoco()
    {
        _poco = new ItemInfoPoco(existentItemID, "testItem", "testDescription", 99, 2300, 20, 9922);
    }
}
