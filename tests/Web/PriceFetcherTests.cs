using System.Text.Json;
using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class PriceFetcherTests
{
    private PriceFetcher _fetcher;
    private MockHttpMessageHandler _handler;
    private HttpClient _client;
    private ILogger<PriceFetcher> _logger;

    [SetUp]
    public void SetUp()
    {
        _handler = new MockHttpMessageHandler();
        _handler.When(_fullPathSingle).Respond(_contentType, _getItemJSONResponseSingle);
        _handler.When(_fullPathMulti).Respond(_contentType, _getItemJSONResponseMulti);

        _client = _handler.ToHttpClient();
        _logger = Substitute.For<ILogger<PriceFetcher>>();
        _fetcher = new PriceFetcher(_client, _logger);
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
        var result = await _fetcher.FetchPriceAsync(_worldID, _itemID1);

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
        var result = JsonSerializer.Deserialize<PriceWebResponse>(
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
        var result = await _fetcher.FetchMultiplePricesAsync(_worldID, ids);

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
    public async Task WhenWeGetAllMarketableItemIds_ThenWeFetchThemToAPI()
    {
        var marketableRequests = _handler
            .When("*marketable*")
            .Respond(_contentType, _getItemJSONResponseMarketable);

        await _fetcher.GetMarketableItemIDsAsync();

        var callCountMarketable = _handler.GetMatchCount(marketableRequests);
        Assert.That(callCountMarketable, Is.EqualTo(1));
    }

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };

    private static readonly string _contentType = "application/json";
    private static readonly int _worldID = 34;
    private static readonly int _itemID1 = 4211;
    private static readonly int _itemID2 = 4222;

    private static readonly string _getItemJSONResponseMarketable = """
[2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,1601,1602,1603,1604,1605,1606,1607,1609,1611,1613,1614,1616,1621,1622,1625,1627,1633,1635,1636,1637,1639,1642,1643,1648,1649,1650,1657,1659,1662,1663,1666,1670,1673,1680,1681,1682,1683,1684,1685,1686,1688,1694,1697,1699,1701,1706,1708,1711,1716,1723,1728,1731,1732,1733,1736,1740,1743,1749,1750,1751,1752,1753,1754,1756,1758,1764,1766,1769,1771,1776,1778,1781,1786,1796,1799,1800,1801,1803,1806,1810,1813,1819,1820,1821,1822,1823,1824,1825,1827,1833,1836,1838,1840,1845,1847,1850,1855,1862,1867,1870,1871,1872,1875,1879,1882,1889,1891,1892,1893,1894,1895,1897,1899,1905,1908,1910,1915,1917,1920,1925,1932,1937,1940,1941,1942,1945,1949,1953,1958,1959,1960,1961,1963,1965,1967,1969,1971,1974,1976,1978,1981,1983,1985,1987,1995,1996,1997,1998,2000,2002,2004,2010,2011,2014,2015,2021,2023,2026,2030,2039,2041,37402,37403,37404,37405,37408,37409,37410,37411,37412,37413,37416,37417,37418,37430,37431,37432,37433,37434,37435,37437,37441,37484,37485,37487,37488,37489,37490,37491,37492,37693,37696,37698,37699,37700,37701,37742,37743,37744,37745,37746,37747,37748,37749,37750,37751,37752,37753,37754,37755,37756,37757,37758,37759,37760,37761,37762,37763,37764,37765,37766,37767,37768,37769,37770,37771,37772,37773,37774,37775,37776,37777,37778,37779,37780,37781,37782,37783,37784,37785,37786,37787,37788,37789,37790,37791,37792,37793,37794,37795,37796,37797,37798,37799,37800,37801,37802,37803,37804,37805,37806,37807,37808,37809,37810,37811,37812,37813,37814,37815,37816,37817,37818,37819,37820,37821,37822,37823,37825,37826,37827,37828,37829,37830,37831,37832,37833,37834,37835,37836,37837,37838,37839,37840,37841,37842,37843,37844,37845,37846,37847,37848,37849,37850,37851,37852,37853,38223,38224,38225,38226,38227,38243,38244,38245,38246,38247,38261,38262,38263,38264,38265,38266,38267,38268,38269,38270,38271,38272,38273,38274,38275,38421,38422,38423,38424,38425,38426,38427,38428,38440,38448,38450,38451,38452,38453,38457,38461,38536,38537,38539,38540,38541,38542,38543,38544,38545,38546,38547,38548,38549,38550,38551,38552,38553,38554,38555,38556,38557,38558,38560,38561,38562,38563,38564,38565,38566,38567,38568,38570,38573,38576,38577,38578,38580,38582,38583,38585,38586,38587,38589,38591,38592,38593,38594,38595,38596,38597,38598,38600,38601,38602,38605,38606,38607,38608,38609,38610,38612,38616,38617,38618,38619,38620,38621,38622,38623,38625,38626,38628,38629,38630,38631,38632,38633,38634,38635,38636,38640,38641,38642,38643,38644,38645,38646,38647,38648,38649,38652,38653,38657,38658,38659,38660,38661,38662,38663,38664,38665,38666,38667,38668,38672,38673,38674,38675,38684,38685,38686,38830,38831,38832,38833,38834,38835,38836,38837,38838,38839,38840,38841,38890,38891,38892,38893,38894,38895,38896,38897,38898,38899,38900,38901,38902,38903,38904,38905,38906,38907,38908,38909,38910,38911,38912,38913,38914,38915,38916,38917,38918,38919,38920,38921,38922,38923,38924,38925,38926,38927,38928,38929,38930,38931,38932,38933,38934,38935,38936,38937,38938,38939,38953,38954,38955,38956,39236,39239,39241,39242,39243,39244,39308,39309,39310,39311,39312,39313,39314,39315,39316,39317,39318,39319,39320,39321,39322,39323,39324,39379,39380,39381,39382,39384,39385,39386,39387,39390,39392,39393,39394,39395,39401,39402,39403,39404,39405,39406,39407,39408,39409,39410,39411,39412,39413,39414,39415,39416,39419,39421,39422,39423,39424,39425,39427,39428,39429,39430,39431,39460,39461,39462,39463,39464,39465,39466,39467,39468,39469,39470,39474,39476,39478,39482,39484,39490,39491,39493,39494,39501,39502,39549,39550,39551,39552,39553,39554,39555,39556,39557,39558,39559,39560,39561,39562,39563,39564,39565,39566,39567,39582,39583,39585,39586,39587,39588,39589,39590,39591,39592,39595,39596,39598,39600,39601,39602,39603,39604,39605,39606,39607,39608,39609,39610,39616,39617,39618]
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
