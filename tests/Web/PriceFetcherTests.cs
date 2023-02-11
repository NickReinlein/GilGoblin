using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class PriceFetcherTests
{
    private PriceFetcher _fetcher;
    private MockHttpMessageHandler _handler;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        // _handler = new MockHttpMessageHandler();
        // _handler.When(_fullPathSingle).Respond(_contentType, _getItemJSONResponseSingle);
        // _handler.When(_fullPathMulti).Respond(_contentType, _getItemJSONResponseMulti);
        // _client = _handler.ToHttpClient();
        _client = new HttpClient();
        _fetcher = new PriceFetcher(_client);
    }

    [Test]
    public async Task WhenWeGetAPrice_WeParseTheResponseSuccessfully()
    {
        var result = await _fetcher.Get(_worldID, _itemID1);

        Assert.That(result?.ItemID, Is.EqualTo(_itemID1));
        Assert.That(result.WorldID, Is.EqualTo(_worldID));
        Assert.That(result.AverageListingPrice, Is.GreaterThan(0));
        Assert.That(result.AverageListingPriceHQ, Is.GreaterThan(0));
        Assert.That(result.AverageListingPriceNQ, Is.GreaterThan(0));
        Assert.That(result.AverageSold, Is.GreaterThan(0));
        Assert.That(result.AverageSoldHQ, Is.GreaterThan(0));
        Assert.That(result.AverageSoldNQ, Is.GreaterThan(0));
    }

    private static readonly string _contentType = "application/json";
    private static readonly int _worldID = 34;
    private static readonly int _itemID1 = 5059;
    private static readonly int _itemID2 = 5060;

    private static readonly string _getItemJSONResponseMulti = $$$$"""
{"items":{"5059":{"itemID":5059,"worldID":34,"lastUploadTime":1676137718113,"currentAveragePrice":1159,"currentAveragePriceNQ":1130.909,"currentAveragePriceHQ":1313.5,"averagePrice":999.85,"averagePriceNQ":999.8,"averagePriceHQ":1000},"5060":{"itemID":5060,"worldID":34,"lastUploadTime":1676122435109,"currentAveragePrice":7901.206,"currentAveragePriceNQ":7926.7036,"currentAveragePriceHQ":7802.857,"averagePrice":6869.8,"averagePriceNQ":6738.923,"averagePriceHQ":7112.857}}}
""";

    private static readonly string _getItemJSONResponseSingle = $$"""
{"itemID":5059,"worldID":34,"lastUploadTime":1676137718113,"currentAveragePrice":1159,"currentAveragePriceNQ":1130.909,"currentAveragePriceHQ":1313.5,"averagePrice":999.85,"averagePriceNQ":999.8,"averagePriceHQ":1000}
""";

    private static readonly string _fullPathMulti = $$"""
https://universalis.app/api/v2/34/5050,5060?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";

    private static readonly string _fullPathSingle = $$"""
https://universalis.app/api/v2/34/5059?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
