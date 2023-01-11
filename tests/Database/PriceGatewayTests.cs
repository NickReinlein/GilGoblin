using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class PriceGatewayTests
{
    private readonly IPriceGateway _gateway = Substitute.For<IPriceGateway>();
    private readonly int[] _itemIDs = { 1, 2, 3, 4, 5, 6 };
    private readonly PricePoco _poco =
        new(1, 1, 1, "test", "testRealm", 300, 200, 400, 600, 400, 800);
    private PricePoco[] _pocos = Array.Empty<PricePoco>();

    [SetUp]
    public void SetUp()
    {
        _pocos = new PricePoco[] { _poco };
    }

    [TearDown]
    public void TearDown()
    {
        _gateway.ClearReceivedCalls();
    }

    // [Test]
    // public void GivenAPriceGateway_WhenFetchingItems_WhenSucessfull_ThenItemsAreReturned()
    // {
    //     MockFetch<IEnumerable<PricePoco>>(_pocos);

    //     var result = _gateway.FetchMarketData(1, _itemIDs);

    //     _gateway.Received().FetchMarketData(Arg.Any<int>(), Arg.Any<int[]>());
    //     Assert.That(result, Is.EquivalentTo(_pocos));
    // }

    // [Test]
    // public void GivenAPriceGateway_WhenFetchingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
    // {
    //     var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
    //     var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
    //     var itemsWithoutDupes = ConvertItemIDsToPocos(itemIdListWithoutDupes);
    //     _gateway.FetchMarketData(Arg.Any<int>(), itemIdListWithDupes).Returns(itemsWithoutDupes);

    //     var result = _gateway.FetchMarketData(1, itemIdListWithDupes);

    //     Assert.That(result, Is.EqualTo(itemsWithoutDupes));
    // }

    // [Test]
    // public void GivenAPriceGateway_WhenFetchingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
    // {
    //     MockFetch<IEnumerable<PricePoco>>(Array.Empty<PricePoco>());

    //     var result = _gateway.FetchMarketData(1, _itemIDs);

    //     _gateway.Received().FetchMarketData(Arg.Any<int>(), Arg.Any<int[]>());
    //     Assert.That(result, Is.Empty);
    // }

    // [Test]
    // public void GivenAPriceGateway_WhenFetchingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
    // {
    //     var poco1 = new PricePoco(_poco) { ItemID = 444 };
    //     var poco2 = new PricePoco(_poco) { ItemID = 777 };
    //     var existingItems = new PricePoco[] { poco1, poco2 };
    //     MockFetch<IEnumerable<PricePoco>>(existingItems);
    //     Assume.That(_itemIDs.Length, Is.GreaterThan(existingItems.Length));

    //     var result = _gateway.FetchMarketData(1, _itemIDs);

    //     _gateway.Received().FetchMarketData(1, _itemIDs);
    //     Assert.That(result, Is.EquivalentTo(existingItems));
    // }

    // [Test]
    // public void GivenAPriceGateway_WhenGettingItems_WhenSucessfull_ThenItemsAreReturned()
    // {
    //     MockFetch<IEnumerable<PricePoco>>(_pocos);

    //     var result = _gateway.FetchMarketData(1, _itemIDs);

    //     _gateway.Received().FetchMarketData(Arg.Any<int>(), Arg.Any<int[]>());
    //     Assert.That(result, Is.EquivalentTo(_pocos));
    // }

    [Test]
    public void GivenAPriceGateway_WhenGettingItems_WhenSuccessfulWithDuplicateIDs_ThenItemsAreReturnedWithoutDuplicates()
    {
        var itemIdListWithDupes = new int[] { 1, 1, 1, 2, 2, 3 };
        var itemIdListWithoutDupes = itemIdListWithDupes.Distinct().ToArray();
        var itemsWithoutDupes = ConvertItemIDsToPocos(itemIdListWithoutDupes);
        _gateway.GetPrices(Arg.Any<int>(), itemIdListWithDupes).Returns(itemsWithoutDupes);

        var result = _gateway.GetPrices(1, itemIdListWithDupes);

        Assert.That(result, Is.EqualTo(itemsWithoutDupes));
    }

    [Test]
    public void GivenAPriceGateway_WhenGettingItems_WhenItemDoesNotExist_ThenNothingIsReturned()
    {
        MockFetch<IEnumerable<PricePoco>>(Array.Empty<PricePoco>());

        var result = _gateway.GetPrices(1, _itemIDs);

        _gateway.Received().GetPrices(Arg.Any<int>(), Arg.Any<int[]>());
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GivenAPriceGateway_WhenGettingItems_WhenSomeAreSuccessfulAndSomeFail_ThenSuccessfulAreReturned()
    {
        var poco1 = new PricePoco(_poco) { ItemID = 444 };
        var poco2 = new PricePoco(_poco) { ItemID = 777 };
        var existingItems = new PricePoco[] { poco1, poco2 };
        MockFetch<IEnumerable<PricePoco>>(existingItems);
        Assume.That(_itemIDs.Length, Is.GreaterThan(existingItems.Length));

        var result = _gateway.GetPrices(1, _itemIDs);

        _gateway.Received().GetPrices(1, _itemIDs);
        Assert.That(result, Is.EquivalentTo(existingItems));
    }

    private void MockFetch<T>(IEnumerable<PricePoco> pocosReturned)
    {
        _gateway.GetPrices(Arg.Any<int>(), Arg.Any<IEnumerable<int>>()).Returns(pocosReturned);
    }

    private static IEnumerable<PricePoco> ConvertItemIDsToPocos(IEnumerable<int> itemIDs)
    {
        return itemIDs.Select(x => new PricePoco(x, 1, 1, "fake", "fakeRealm", 3, 2, 4, 6, 4, 8));
    }
}
