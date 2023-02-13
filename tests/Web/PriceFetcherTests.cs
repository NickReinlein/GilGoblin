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
        var result = JsonSerializer.Deserialize<List<PriceWebPoco>>(
            _getItemJSONResponseMulti,
            GetSerializerOptions()
        );

        foreach (var price in result)
        {
            Assert.Multiple(() =>
            {
                // Assert.That((new int[] { _itemID1, _itemID2 }).Contains(price.ItemID), Is.True);
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

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.GreaterThan(0));
        foreach (var price in result)
        {
            Assert.Multiple(() =>
            {
                // Assert.That((new int[] { _itemID1, _itemID2 }).Contains(price.ItemID), Is.True);
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

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };

    private static readonly string _contentType = "application/json";
    private static readonly int _worldID = 34;
    private static readonly int _itemID1 = 5059;
    private static readonly int _itemID2 = 5060;

    private static readonly string _getItemJSONResponseMulti = $$$$"""
{"items":{"5059":{"itemID":5059,"worldID":34,"lastUploadTime":1676225135585,"currentAveragePrice":1015.2222,"currentAveragePriceNQ":1004.7,"currentAveragePriceHQ":1067.8334,"averagePrice":763.2,"averagePriceNQ":748.17645,"averagePriceHQ":848.3333},"5060":{"itemID":5060,"worldID":34,"lastUploadTime":1676227379362,"currentAveragePrice":8422.68,"currentAveragePriceNQ":8596.647,"currentAveragePriceHQ":8053,"averagePrice":6693.4,"averagePriceNQ":6693.4,"averagePriceHQ":0}}}
""";
    private static readonly string _getItemJSONResponseSingle = $$"""
{"itemID":5059,"worldID":34,"lastUploadTime":1676137718113,"currentAveragePrice":1159,"currentAveragePriceNQ":1130.909,"currentAveragePriceHQ":1313.5,"averagePrice":999.85,"averagePriceNQ":999.8,"averagePriceHQ":1000}
""";
    private static readonly string _fullPathMulti = $"""
https://universalis.app/api/v2/34/5059,5060?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";
    private static readonly string _fullPathSingle = $"""
https://universalis.app/api/v2/34/5059?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
