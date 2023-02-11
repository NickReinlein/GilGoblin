using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Web;

public class PriceFetcherTests
{
    private readonly PriceFetcher _fetcher;

    public PriceFetcherTests()
    {
        _fetcher = new PriceFetcher();
    }

    [SetUp]
    public void SetUp()
    {
        _fetcher
            .GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>())
            .Returns(_getItemJSONResponse);
    }

    [TearDown]
    public void TearDown() { }

    [Test]
    public async Task WhenWeGetAnItem_WeCallUsingCorrectParameters()
    {
        var result = await fetcher.GetAsync(_getPath);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ID, Is.EqualTo(int.Parse(_getPath)));
    }

    [Test]
    public async Task WhenWeGetAllItems_WeCallUsingCorrectParameters()
    {
        var result = await fetcher.GetAllAsync(_getAllPath);

        Assert.That(result, Is.Not.Null);
    }

    private static readonly string _fullPathGet = string.Concat(
        ItemInfoFetcher.ItemInfoBaseUrl,
        _getPath
    );

    private static readonly string _fullPathGetAll = string.Concat(
        ItemInfoFetcher.ItemInfoBaseUrl,
        _getAllPath
    );
    private static readonly string _getPath = """1""";
    private static readonly string _getAllPath = string.Empty;

    private static readonly string _getItemJSONResponse = $$"""
{
{
  "items": {
    "5050": {
      "itemID": 5050,
      "worldID": 34,
      "currentAveragePrice": 5.375,
      "currentAveragePriceNQ": 5.375,
      "currentAveragePriceHQ": 0,
      "averagePrice": 9.8,
      "averagePriceNQ": 9.8,
      "averagePriceHQ": 0
    },
    "5060": {
      "itemID": 5060,
      "worldID": 34,
      "currentAveragePrice": 8234.064,
      "currentAveragePriceNQ": 8309.25,
      "currentAveragePriceHQ": 7976.2856,
      "averagePrice": 6994.85,
      "averagePriceNQ": 7050.357,
      "averagePriceHQ": 6865.3335
    }
  }
}
""";

    private static readonly string _fullPath =
        $$"""https://universalis.app/api/v2/34/5050,5060?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ""";
}
