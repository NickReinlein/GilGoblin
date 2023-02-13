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
    public void WhenWeDeserializeResponseForMarketable_ThenWeSucceed()
    {
        var result = JsonSerializer.Deserialize<List<int>>(
            _getItemJSONResponseMarketable,
            GetSerializerOptions()
        );

        Assert.That(result.Count, Is.GreaterThan(50));
        Assert.That(result.All(i => i > 0), Is.True);
    }

    [Test]
    public async Task WhenWeGetAllPrices_ThenWeFetchAllMarketableItemIds()
    {
        var marketableRequests = _handler
            .When("*marketable*")
            .Respond(_contentType, _getItemJSONResponseMarketable);

        var itemRequests = _handler
            .When("*/api/v2/34/*")
            .Respond(_contentType, _getItemJSONResponseMulti);

        await _fetcher.GetAll(_worldID);

        var callCountMarketable = _handler.GetMatchCount(marketableRequests);
        var callCountItems = _handler.GetMatchCount(itemRequests);
        Assert.Multiple(() =>
        {
            Assert.That(callCountMarketable, Is.EqualTo(1));
            Assert.That(callCountItems, Is.GreaterThanOrEqualTo(1));
        });
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };

    private static readonly string _contentType = "application/json";
    private static readonly int _worldID = 34;
    private static readonly int _itemID1 = 4211;
    private static readonly int _itemID2 = 4222;

    private static readonly string _getItemJSONResponseMarketable = """
[2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,1601,1602,1603,1604,1605,1606,1607,1609,1611,1613,1614,1616,1621,39558,39559,39560,39561,39562,39563,39564,39565,39566,39567,39582,39583,39585,39586,39587,39588,39589,39590,39591,39592,39595,39596,39598,39600,39601,39602,39603,39604,39605,39606,39607,39608,39609,39610,39616,39617,39618]
""";

    private static readonly string _getItemJSONResponseMulti = $$$$"""
{"items":{"4211":{"itemID":4211,"worldID":34,"lastUploadTime":1676094405639,"currentAveragePrice":15752.5,"currentAveragePriceNQ":10504.333,"currentAveragePriceHQ":31497,"averagePrice":16676.4,"averagePriceNQ":13321.454,"averagePriceHQ":20776.889},"4222":{"itemID":4222,"worldID":34,"lastUploadTime":1675869178624,"currentAveragePrice":5986.5713,"currentAveragePriceNQ":2084.75,"currentAveragePriceHQ":11189,"averagePrice":1631.45,"averagePriceNQ":1652.909,"averagePriceHQ":1605.2222}}}
""";

    private static readonly string _getItemJSONResponseSingle = $$"""
{"itemID":4211,"worldID":34,"lastUploadTime":1676137718113,"currentAveragePrice":1159,"currentAveragePriceNQ":1130.909,"currentAveragePriceHQ":1313.5,"averagePrice":999.85,"averagePriceNQ":999.8,"averagePriceHQ":1000}
""";

    private static readonly string _fullPathMarketableIds =
        """https://universalis.app/api/v2/marketable""";

    private static readonly string _fullPathMulti = $"""
https://universalis.app/api/v2/34/4211,4222?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";

    private static readonly string _fullPathSingle = $"""
https://universalis.app/api/v2/34/4211?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
    private static readonly string _getItemJSONResponseMany = $$$$"""
{"items":{"2":{"itemID":2,"worldID":34,"currentAveragePrice":207.82608,"currentAveragePriceNQ":207.82608,"currentAveragePriceHQ":0,"averagePrice":81.052635,"averagePriceNQ":81.052635,"averagePriceHQ":0},"3":{"itemID":3,"worldID":34,"currentAveragePrice":177.10345,"currentAveragePriceNQ":177.10345,"currentAveragePriceHQ":0,"averagePrice":99.3,"averagePriceNQ":99.3,"averagePriceHQ":0},"4":{"itemID":4,"worldID":34,"currentAveragePrice":146.97144,"currentAveragePriceNQ":146.97144,"currentAveragePriceHQ":0,"averagePrice":78.55,"averagePriceNQ":78.55,"averagePriceHQ":0},"5":{"itemID":5,"worldID":34,"currentAveragePrice":146.31818,"currentAveragePriceNQ":146.31818,"currentAveragePriceHQ":0,"averagePrice":49.65,"averagePriceNQ":49.65,"averagePriceHQ":0},"6":{"itemID":6,"worldID":34,"currentAveragePrice":109,"currentAveragePriceNQ":109,"currentAveragePriceHQ":0,"averagePrice":60.4,"averagePriceNQ":60.4,"averagePriceHQ":0},"7":{"itemID":7,"worldID":34,"currentAveragePrice":275.9375,"currentAveragePriceNQ":275.9375,"currentAveragePriceHQ":0,"averagePrice":76.210526,"averagePriceNQ":76.210526,"averagePriceHQ":0},"8":{"itemID":8,"worldID":34,"currentAveragePrice":95.03571,"currentAveragePriceNQ":95.03571,"currentAveragePriceHQ":0,"averagePrice":53.85,"averagePriceNQ":53.85,"averagePriceHQ":0},"9":{"itemID":9,"worldID":34,"currentAveragePrice":556.0345,"currentAveragePriceNQ":556.0345,"currentAveragePriceHQ":0,"averagePrice":28.263159,"averagePriceNQ":28.263159,"averagePriceHQ":0},"10":{"itemID":10,"worldID":34,"currentAveragePrice":262.5,"currentAveragePriceNQ":262.5,"currentAveragePriceHQ":0,"averagePrice":66.05,"averagePriceNQ":66.05,"averagePriceHQ":0},"11":{"itemID":11,"worldID":34,"currentAveragePrice":154.5,"currentAveragePriceNQ":154.5,"currentAveragePriceHQ":0,"averagePrice":14.75,"averagePriceNQ":14.75,"averagePriceHQ":0},"12":{"itemID":12,"worldID":34,"currentAveragePrice":3598.518,"currentAveragePriceNQ":3598.518,"currentAveragePriceHQ":0,"averagePrice":40.789474,"averagePriceNQ":40.789474,"averagePriceHQ":0},"13":{"itemID":13,"worldID":34,"currentAveragePrice":5704.8,"currentAveragePriceNQ":5704.8,"currentAveragePriceHQ":0,"averagePrice":43.2,"averagePriceNQ":43.2,"averagePriceHQ":0},"14":{"itemID":14,"worldID":34,"currentAveragePrice":1018.11426,"currentAveragePriceNQ":1018.11426,"currentAveragePriceHQ":0,"averagePrice":75.95,"averagePriceNQ":75.95,"averagePriceHQ":0},"15":{"itemID":15,"worldID":34,"currentAveragePrice":207.43182,"currentAveragePriceNQ":207.43182,"currentAveragePriceHQ":0,"averagePrice":13.05,"averagePriceNQ":13.05,"averagePriceHQ":0},"16":{"itemID":16,"worldID":34,"currentAveragePrice":244,"currentAveragePriceNQ":244,"currentAveragePriceHQ":0,"averagePrice":34.7,"averagePriceNQ":34.7,"averagePriceHQ":0},"17":{"itemID":17,"worldID":34,"currentAveragePrice":109.32353,"currentAveragePriceNQ":109.32353,"currentAveragePriceHQ":0,"averagePrice":38.3,"averagePriceNQ":38.3,"averagePriceHQ":0},"18":{"itemID":18,"worldID":34,"currentAveragePrice":173.61539,"currentAveragePriceNQ":173.61539,"currentAveragePriceHQ":0,"averagePrice":77.1579,"averagePriceNQ":77.1579,"averagePriceHQ":0},"19":{"itemID":19,"worldID":34,"currentAveragePrice":259.48276,"currentAveragePriceNQ":259.48276,"currentAveragePriceHQ":0,"averagePrice":39.55,"averagePriceNQ":39.55,"averagePriceHQ":0},"1601":{"itemID":1601,"worldID":34,"currentAveragePrice":1154.9,"currentAveragePriceNQ":1154.9,"currentAveragePriceHQ":0,"averagePrice":1552.4736,"averagePriceNQ":1552.4736,"averagePriceHQ":0},"1602":{"itemID":1602,"worldID":34,"currentAveragePrice":1544.8572,"currentAveragePriceNQ":1544.8572,"currentAveragePriceHQ":0,"averagePrice":2148656.5,"averagePriceNQ":2551138,"averagePriceHQ":2087.6667},"1603":{"itemID":1603,"worldID":34,"currentAveragePrice":580,"currentAveragePriceNQ":105,"currentAveragePriceHQ":1055,"averagePrice":2458.6316,"averagePriceNQ":1186.5,"averagePriceHQ":3045.7693},"1604":{"itemID":1604,"worldID":34,"currentAveragePrice":3269.7144,"currentAveragePriceNQ":282,"currentAveragePriceHQ":5510.5,"averagePrice":692.5263,"averagePriceNQ":315,"averagePriceHQ":1211.625},"1605":{"itemID":1605,"worldID":34,"currentAveragePrice":21000,"currentAveragePriceNQ":0,"currentAveragePriceHQ":21000,"averagePrice":8923.7,"averagePriceNQ":3497.25,"averagePriceHQ":17063.375},"1606":{"itemID":1606,"worldID":34,"currentAveragePrice":0,"currentAveragePriceNQ":0,"currentAveragePriceHQ":0,"averagePrice":5168.6,"averagePriceNQ":5168.6,"averagePriceHQ":0},"1607":{"itemID":1607,"worldID":34,"currentAveragePrice":2126.75,"currentAveragePriceNQ":2126.75,"currentAveragePriceHQ":0,"averagePrice":293.3684,"averagePriceNQ":228.35715,"averagePriceHQ":1229.5},"1609":{"itemID":1609,"worldID":34,"currentAveragePrice":4190,"currentAveragePriceNQ":4190,"currentAveragePriceHQ":0,"averagePrice":4745,"averagePriceNQ":3900,"averagePriceHQ":5026.6665},"1611":{"itemID":1611,"worldID":34,"currentAveragePrice":825.6429,"currentAveragePriceNQ":851.4,"currentAveragePriceHQ":761.25,"averagePrice":462,"averagePriceNQ":356.75,"averagePriceHQ":837.125},"1613":{"itemID":1613,"worldID":34,"currentAveragePrice":6305274,"currentAveragePriceNQ":6305274,"currentAveragePriceHQ":0,"averagePrice":589.2105,"averagePriceNQ":589.2105,"averagePriceHQ":0},"1614":{"itemID":1614,"worldID":34,"currentAveragePrice":519.8,"currentAveragePriceNQ":519.8,"currentAveragePriceHQ":0,"averagePrice":2350.15,"averagePriceNQ":1618.8889,"averagePriceHQ":2948.4546},"1616":{"itemID":1616,"worldID":34,"currentAveragePrice":34124.5,"currentAveragePriceNQ":34124.5,"currentAveragePriceHQ":0,"averagePrice":8807.947,"averagePriceNQ":8807.947,"averagePriceHQ":0},"1621":{"itemID":1621,"worldID":34,"currentAveragePrice":3762.6667,"currentAveragePriceNQ":3762.6667,"currentAveragePriceHQ":0,"averagePrice":10982.2,"averagePriceNQ":10982.2,"averagePriceHQ":0},"39558":{"itemID":39558,"worldID":34,"currentAveragePrice":997500,"currentAveragePriceNQ":0,"currentAveragePriceHQ":997500,"averagePrice":732710.56,"averagePriceNQ":723074.5,"averagePriceHQ":733781.25},"39559":{"itemID":39559,"worldID":34,"currentAveragePrice":941481.8,"currentAveragePriceNQ":0,"currentAveragePriceHQ":941481.8,"averagePrice":633998.9,"averagePriceNQ":0,"averagePriceHQ":633998.9},"39560":{"itemID":39560,"worldID":34,"currentAveragePrice":0,"currentAveragePriceNQ":0,"currentAveragePriceHQ":0,"averagePrice":672376.06,"averagePriceNQ":0,"averagePriceHQ":672376.06},"39561":{"itemID":39561,"worldID":34,"currentAveragePrice":524999,"currentAveragePriceNQ":0,"currentAveragePriceHQ":524999,"averagePrice":637856.4,"averagePriceNQ":0,"averagePriceHQ":637856.4},"39562":{"itemID":39562,"worldID":34,"currentAveragePrice":797980,"currentAveragePriceNQ":0,"currentAveragePriceHQ":797980,"averagePrice":566793.5,"averagePriceNQ":0,"averagePriceHQ":566793.5},"39563":{"itemID":39563,"worldID":34,"currentAveragePrice":945000,"currentAveragePriceNQ":0,"currentAveragePriceHQ":945000,"averagePrice":564654.9,"averagePriceNQ":788709,"averagePriceHQ":552862.5},"39564":{"itemID":39564,"worldID":34,"currentAveragePrice":787482.3,"currentAveragePriceNQ":0,"currentAveragePriceHQ":787482.3,"averagePrice":617628.2,"averagePriceNQ":450000,"averagePriceHQ":626940.8},"39565":{"itemID":39565,"worldID":34,"currentAveragePrice":1063124.8,"currentAveragePriceNQ":0,"currentAveragePriceHQ":1063124.8,"averagePrice":744597.9,"averagePriceNQ":0,"averagePriceHQ":744597.9},"39566":{"itemID":39566,"worldID":34,"currentAveragePrice":734998.5,"currentAveragePriceNQ":0,"currentAveragePriceHQ":734998.5,"averagePrice":454528.34,"averagePriceNQ":0,"averagePriceHQ":454528.34},"39567":{"itemID":39567,"worldID":34,"currentAveragePrice":0,"currentAveragePriceNQ":0,"currentAveragePriceHQ":0,"averagePrice":594443.75,"averagePriceNQ":594443.75,"averagePriceHQ":0},"39582":{"itemID":39582,"worldID":34,"currentAveragePrice":38276.79,"currentAveragePriceNQ":38276.79,"currentAveragePriceHQ":0,"averagePrice":2815.2632,"averagePriceNQ":2815.2632,"averagePriceHQ":0},"39583":{"itemID":39583,"worldID":34,"currentAveragePrice":561090.56,"currentAveragePriceNQ":561090.56,"currentAveragePriceHQ":0,"averagePrice":421986.62,"averagePriceNQ":421986.62,"averagePriceHQ":0},"39585":{"itemID":39585,"worldID":34,"currentAveragePrice":682848.8,"currentAveragePriceNQ":682848.8,"currentAveragePriceHQ":0,"averagePrice":795763.8,"averagePriceNQ":795763.8,"averagePriceHQ":0},"39586":{"itemID":39586,"worldID":34,"currentAveragePrice":1066037.1,"currentAveragePriceNQ":1066037.1,"currentAveragePriceHQ":0,"averagePrice":913791.2,"averagePriceNQ":913791.2,"averagePriceHQ":0},"39587":{"itemID":39587,"worldID":34,"currentAveragePrice":2893543,"currentAveragePriceNQ":2893543,"currentAveragePriceHQ":0,"averagePrice":1639972.2,"averagePriceNQ":1639972.2,"averagePriceHQ":0},"39588":{"itemID":39588,"worldID":34,"currentAveragePrice":1511522.9,"currentAveragePriceNQ":1511522.9,"currentAveragePriceHQ":0,"averagePrice":2231497.2,"averagePriceNQ":2231497.2,"averagePriceHQ":0},"39589":{"itemID":39589,"worldID":34,"currentAveragePrice":70927.734,"currentAveragePriceNQ":70927.734,"currentAveragePriceHQ":0,"averagePrice":31282.684,"averagePriceNQ":31282.684,"averagePriceHQ":0},"39590":{"itemID":39590,"worldID":34,"currentAveragePrice":7220749,"currentAveragePriceNQ":7220749,"currentAveragePriceHQ":0,"averagePrice":6726302.5,"averagePriceNQ":6726302.5,"averagePriceHQ":0},"39591":{"itemID":39591,"worldID":34,"currentAveragePrice":39916.07,"currentAveragePriceNQ":39916.07,"currentAveragePriceHQ":0,"averagePrice":15406.95,"averagePriceNQ":15406.95,"averagePriceHQ":0},"39592":{"itemID":39592,"worldID":34,"currentAveragePrice":3172436.2,"currentAveragePriceNQ":3172436.2,"currentAveragePriceHQ":0,"averagePrice":1994196,"averagePriceNQ":1994196,"averagePriceHQ":0},"39595":{"itemID":39595,"worldID":34,"currentAveragePrice":32965.51,"currentAveragePriceNQ":32965.51,"currentAveragePriceHQ":0,"averagePrice":13997.421,"averagePriceNQ":13997.421,"averagePriceHQ":0},"39596":{"itemID":39596,"worldID":34,"currentAveragePrice":431063.16,"currentAveragePriceNQ":431063.16,"currentAveragePriceHQ":0,"averagePrice":81638.25,"averagePriceNQ":81638.25,"averagePriceHQ":0},"39598":{"itemID":39598,"worldID":34,"currentAveragePrice":46250.79,"currentAveragePriceNQ":46250.79,"currentAveragePriceHQ":0,"averagePrice":25848.895,"averagePriceNQ":25848.895,"averagePriceHQ":0},"39600":{"itemID":39600,"worldID":34,"currentAveragePrice":77177.7,"currentAveragePriceNQ":77177.7,"currentAveragePriceHQ":0,"averagePrice":11761.45,"averagePriceNQ":11761.45,"averagePriceHQ":0},"39601":{"itemID":39601,"worldID":34,"currentAveragePrice":100778.44,"currentAveragePriceNQ":100778.44,"currentAveragePriceHQ":0,"averagePrice":14581.35,"averagePriceNQ":14581.35,"averagePriceHQ":0},"39602":{"itemID":39602,"worldID":34,"currentAveragePrice":279288.97,"currentAveragePriceNQ":279288.97,"currentAveragePriceHQ":0,"averagePrice":68384.8,"averagePriceNQ":68384.8,"averagePriceHQ":0},"39603":{"itemID":39603,"worldID":34,"currentAveragePrice":105392.04,"currentAveragePriceNQ":105392.04,"currentAveragePriceHQ":0,"averagePrice":79365.7,"averagePriceNQ":79365.7,"averagePriceHQ":0},"39604":{"itemID":39604,"worldID":34,"currentAveragePrice":124525.695,"currentAveragePriceNQ":124525.695,"currentAveragePriceHQ":0,"averagePrice":91231.65,"averagePriceNQ":91231.65,"averagePriceHQ":0},"39605":{"itemID":39605,"worldID":34,"currentAveragePrice":556499.1,"currentAveragePriceNQ":556499.1,"currentAveragePriceHQ":0,"averagePrice":533035.5,"averagePriceNQ":533035.5,"averagePriceHQ":0},"39606":{"itemID":39606,"worldID":34,"currentAveragePrice":361606.88,"currentAveragePriceNQ":361606.88,"currentAveragePriceHQ":0,"averagePrice":253283,"averagePriceNQ":253283,"averagePriceHQ":0},"39607":{"itemID":39607,"worldID":34,"currentAveragePrice":115626,"currentAveragePriceNQ":115626,"currentAveragePriceHQ":0,"averagePrice":66675.9,"averagePriceNQ":66675.9,"averagePriceHQ":0},"39608":{"itemID":39608,"worldID":34,"currentAveragePrice":58687.918,"currentAveragePriceNQ":58687.918,"currentAveragePriceHQ":0,"averagePrice":40399.156,"averagePriceNQ":40399.156,"averagePriceHQ":0},"39609":{"itemID":39609,"worldID":34,"currentAveragePrice":788472,"currentAveragePriceNQ":788472,"currentAveragePriceHQ":0,"averagePrice":423829.7,"averagePriceNQ":423829.7,"averagePriceHQ":0},"39610":{"itemID":39610,"worldID":34,"currentAveragePrice":1050000,"currentAveragePriceNQ":1050000,"currentAveragePriceHQ":0,"averagePrice":899166.7,"averagePriceNQ":899166.7,"averagePriceHQ":0},"39616":{"itemID":39616,"worldID":34,"currentAveragePrice":15750,"currentAveragePriceNQ":15750,"currentAveragePriceHQ":0,"averagePrice":14947.105,"averagePriceNQ":14947.105,"averagePriceHQ":0},"39617":{"itemID":39617,"worldID":34,"currentAveragePrice":15750,"currentAveragePriceNQ":15750,"currentAveragePriceHQ":0,"averagePrice":14683.842,"averagePriceNQ":14683.842,"averagePriceHQ":0},"39618":{"itemID":39618,"worldID":34,"currentAveragePrice":839330,"currentAveragePriceNQ":839330,"currentAveragePriceHQ":0,"averagePrice":360161.3,"averagePriceNQ":360161.3,"averagePriceHQ":0}}}
""";
}
