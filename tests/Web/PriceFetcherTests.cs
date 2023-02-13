using System.Text.Json;
using GilGoblin.Pocos;
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
        _handler = new MockHttpMessageHandler();
        _handler.When(_fullPathSingle).Respond(_contentType, _getItemJSONResponseSingle);
        _handler.When(_fullPathMulti).Respond(_contentType, _getItemJSONResponseMulti);
        _client = _handler.ToHttpClient();
        _fetcher = new PriceFetcher(_client);
    }

    [Test]
    public void WhenWeDeserializeResponseForSingle_ThenWeSucceed()
    {
        var result = JsonSerializer.Deserialize<PriceWebPoco>(
            _getItemJSONResponseSingle,
            GetSerializerOptions()
        );

        Assert.That(result.ItemID, Is.EqualTo(_itemID1));
        Assert.That(result.WorldID, Is.EqualTo(_worldID));
        Assert.That(result.CurrentAveragePrice, Is.GreaterThan(0));
        Assert.That(result.CurrentAveragePriceHQ, Is.GreaterThan(0));
        Assert.That(result.CurrentAveragePriceNQ, Is.GreaterThan(0));
        Assert.That(result.AveragePrice, Is.GreaterThan(0));
        Assert.That(result.AveragePriceHQ, Is.GreaterThan(0));
        Assert.That(result.AveragePriceNQ, Is.GreaterThan(0));
    }

    [Test]
    public async Task WhenWeGetAPrice_ThenWeParseSuccessfully()
    {
        var result = await _fetcher.Get(_worldID, _itemID1);

        Assert.Multiple(() =>
        {
            Assert.That(result.ItemID, Is.EqualTo(_itemID1));
            Assert.That(result.WorldID, Is.EqualTo(_worldID));
            Assert.That(result.CurrentAveragePrice, Is.GreaterThan(0));
            Assert.That(result.CurrentAveragePriceHQ, Is.GreaterThan(0));
            Assert.That(result.CurrentAveragePriceNQ, Is.GreaterThan(0));
            Assert.That(result.AveragePrice, Is.GreaterThan(0));
            Assert.That(result.AveragePriceHQ, Is.GreaterThan(0));
            Assert.That(result.AveragePriceNQ, Is.GreaterThan(0));
        });
    }

    [Test]
    public void WhenWeDeserializeResponseForMultiple_ThenWeSucceed()
    {
        var ids = new[] { _itemID1, _itemID2 };
        var result = JsonSerializer.Deserialize<PriceWebResponsePoco>(
            _getItemJSONResponseMulti,
            GetSerializerOptions()
        );

        var prices = result.GetContentAsList();

        Assert.That(prices.Count, Is.GreaterThan(0));
        foreach (var price in prices)
        {
            Assert.Multiple(() =>
            {
                Assert.That(ids, Does.Contain(price.ItemID));
                Assert.That(price.WorldID, Is.EqualTo(_worldID));
                Assert.That(price.CurrentAveragePrice, Is.GreaterThan(0));
                Assert.That(price.CurrentAveragePriceHQ, Is.GreaterThan(0));
                Assert.That(price.CurrentAveragePriceNQ, Is.GreaterThan(0));
                Assert.That(price.AveragePrice, Is.GreaterThan(0));
                Assert.That(price.AveragePriceHQ, Is.GreaterThan(0));
                Assert.That(price.AveragePriceNQ, Is.GreaterThan(0));
            });
        }
    }

    [Test]
    public async Task WhenWeGetMultiplePrices_ThenWeParseSuccessfully()
    {
        var ids = new[] { _itemID1, _itemID2 };
        var result = await _fetcher.GetMultiple(_worldID, ids);

        Assert.That(result.Count, Is.GreaterThan(0));
        foreach (var price in result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(ids, Does.Contain(price.ItemID));
                Assert.That(price.WorldID, Is.EqualTo(_worldID));
                Assert.That(price.CurrentAveragePrice, Is.GreaterThan(0));
                Assert.That(price.CurrentAveragePriceHQ, Is.GreaterThan(0));
                Assert.That(price.CurrentAveragePriceNQ, Is.GreaterThan(0));
                Assert.That(price.AveragePrice, Is.GreaterThan(0));
                Assert.That(price.AveragePriceHQ, Is.GreaterThan(0));
                Assert.That(price.AveragePriceNQ, Is.GreaterThan(0));
            });
        }
    }

    [Test]
    public async Task WhenWeGetAllPrices_ThenWeMakeBatchCalls()
    {
        var request = _handler
            .When("https://universalis.app/api/v2/34/*")
            .Respond("application/json", "{'name' : 'Test Return'}");

        var result = await _fetcher.GetAll(_worldID);

        var count = _handler.GetMatchCount(request);
        Assert.That(count, Is.GreaterThan(2));
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };

    private static readonly string _contentType = "application/json";
    private static readonly int _worldID = 34;
    private static readonly int _itemID1 = 4211;
    private static readonly int _itemID2 = 4222;

    private static readonly string _getItemJSONResponseMulti = $$$$"""
{"items":{"4211":{"itemID":4211,"worldID":34,"lastUploadTime":1676094405639,"currentAveragePrice":15752.5,"currentAveragePriceNQ":10504.333,"currentAveragePriceHQ":31497,"averagePrice":16676.4,"averagePriceNQ":13321.454,"averagePriceHQ":20776.889},"4222":{"itemID":4222,"worldID":34,"lastUploadTime":1675869178624,"currentAveragePrice":5986.5713,"currentAveragePriceNQ":2084.75,"currentAveragePriceHQ":11189,"averagePrice":1631.45,"averagePriceNQ":1652.909,"averagePriceHQ":1605.2222}}}
""";
    private static readonly string _getItemJSONResponseSingle = $$"""
{"itemID":4211,"worldID":34,"lastUploadTime":1676137718113,"currentAveragePrice":1159,"currentAveragePriceNQ":1130.909,"currentAveragePriceHQ":1313.5,"averagePrice":999.85,"averagePriceNQ":999.8,"averagePriceHQ":1000}
""";
    private static readonly string _fullPathMulti = $"""
https://universalis.app/api/v2/34/4211,4222?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";
    private static readonly string _fullPathSingle = $"""
https://universalis.app/api/v2/34/4211?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
