using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Fetcher;

public class MarketableItemIdsFetcherTests : FetcherTests
{
    private MarketableItemIdsFetcher _fetcher;
    private ILogger<MarketableItemIdsFetcher> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<MarketableItemIdsFetcher>>();
        _fetcher = new MarketableItemIdsFetcher(_logger, _client);
    }

    [Test]
    public async Task GivenWeCallGetMarketableIdsAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        var idList = GetIdList();
        _handler
            .When(GetUrl)
            .Respond(HttpStatusCode.OK, ContentType, JsonSerializer.Serialize(idList));

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
        });
    }

    [Test]
    public async Task
        GivenWeCallGetMarketableIdsAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        SetupResponse(success: false, statusCode: HttpStatusCode.NotFound);

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task
        GivenWeCallGetMarketableIdsAsync_WhenTheResponseIsStatusCodeIsSuccessful_ThenWeReturnAnIdList()
    {
        var idList = GetIdList();
        _handler
            .When(GetUrl)
            .Respond(HttpStatusCode.OK, ContentType, JsonSerializer.Serialize(idList));

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.That(result, Is.Not.Empty);
    }

    private static List<int> GetIdList()
    {
        return GetMultipleNewPocos().Select(i => i.Id).ToList();
    }

    [Test]
    public async Task GivenWeCallGetMarketableIdsAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        _handler
            .When(GetUrl)
            .Respond(HttpStatusCode.OK, ContentType, JsonSerializer.Serialize("{asdfasd}"));

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallGetMarketableItemIdsAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        SetupResponse();

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.GreaterThanOrEqualTo(100));
            result.ForEach(i =>
            {
                Assert.That(i, Is.GreaterThan(0));
            });
        });
    }

    [Test]
    public async Task
        GivenWeCallGetMarketableItemIdsAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        SetupResponse(success: false, statusCode: HttpStatusCode.NotFound);

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallGetMarketableItemIdsAsync_WhenTheResponseIsInvalidWith200_ThenWeReturnAnEmptyList()
    {
        SetupResponse(success: false);

        var result = await _fetcher.GetMarketableItemIdsAsync();

        Assert.That(result, Is.Empty);
    }

    private void SetupResponse(bool success = true, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var responseContent
            = success
                ? GetItemsJsonResponseMarketable
                : JsonSerializer.Serialize("garbage text here");

        _handler
            .When(GetUrl)
            .Respond(statusCode, ContentType, responseContent);
    }

    public static IEnumerable<ItemWebPoco> GetMultipleNewPocos()
    {
        var poco1 = new ItemWebPoco { Id = ItemId1 };
        var poco2 = new ItemWebPoco { Id = ItemId2 };
        return new List<ItemWebPoco> { poco1, poco2 };
    }

    private static int ItemId1 => 10972;
    private static int ItemId2 => 10973;
    private static string GetUrl => "https://universalis.app/api/v2/marketable";

    private static string GetItemsJsonResponseMarketable =>
        "[2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,1601,1602,1603,1604,1605,1606,1607,1609,1611,1613,1614,1616,1621,1622,1625,1627,1633,1635,1636,1637,1639,1642,1643,1648,1649,1650,1657,1659,1662,1663,1666,1670,1673,1680,1681,1682,1683,1684,1685,1686,1688,1694,1697,1699,1701,1706,1708,1711,1716,1723,1728,1731,1732,1733,1736,1740,1743,1749,1750,1751,1752,1753,1754,1756,1758,1764,1766,1769,1771,1776,1778,1781,1786,1796,1799,1800,1801,1803,1806,1810,1813,1819,1820,1821,1822,1823,1824,1825,1827,1833,1836,1838,1840,1845,1847,1850,1855,1862,1867,1870,1871,1872,1875,1879,1882,1889,1891,1892,1893,1894,1895,1897,1899,1905,1908,1910,1915,1917,1920,1925,1932,1937,1940,1941,1942,1945,1949,1953,1958,1959,1960,1961,1963,1965,1967,1969,1971,1974,1976,1978,1981,1983,1985,1987,1995,1996,1997,1998,2000,2002,2004,2010,2011,2014,2015,2021,2023,2026,2030,2039,2041,37402,37403,37404,37405,37408,37409,37410,37411,37412,37413,37416,37417,37418,37430,37431,37432,37433,37434,37435,37437,37441,37484,37485,37487,37488,37489,37490,37491,37492,37693,37696,37698,37699,37700,37701,37742,37743,37744,37745,37746,37747,37748,37749,37750,37751,37752,37753,37754,37755,37756,37757,37758,37759,37760,37761,37762,37763,37764,37765,37766,37767,37768,37769,37770,37771,37772,37773,37774,37775,37776,37777,37778,37779,37780,37781,37782,37783,37784,37785,37786,37787,37788,37789,37790,37791,37792,37793,37794,37795,37796,37797,37798,37799,37800,37801,37802,37803,37804,37805,37806,37807,37808,37809,37810,37811,37812,37813,37814,37815,37816,37817,37818,37819,37820,37821,37822,37823,37825,37826,37827,37828,37829,37830,37831,37832,37833,37834,37835,37836,37837,37838,37839,37840,37841,37842,37843,37844,37845,37846,37847,37848,37849,37850,37851,37852,37853,38223,38224,38225,38226,38227,38243,38244,38245,38246,38247,38261,38262,38263,38264,38265,38266,38267,38268,38269,38270,38271,38272,38273,38274,38275,38421,38422,38423,38424,38425,38426,38427,38428,38440,38448,38450,38451,38452,38453,38457,38461,38536,38537,38539,38540,38541,38542,38543,38544,38545,38546,38547,38548,38549,38550,38551,38552,38553,38554,38555,38556,38557,38558,38560,38561,38562,38563,38564,38565,38566,38567,38568,38570,38573,38576,38577,38578,38580,38582,38583,38585,38586,38587,38589,38591,38592,38593,38594,38595,38596,38597,38598,38600,38601,38602,38605,38606,38607,38608,38609,38610,38612,38616,38617,38618,38619,38620,38621,38622,38623,38625,38626,38628,38629,38630,38631,38632,38633,38634,38635,38636,38640,38641,38642,38643,38644,38645,38646,38647,38648,38649,38652,38653,38657,38658,38659,38660,38661,38662,38663,38664,38665,38666,38667,38668,38672,38673,38674,38675,38684,38685,38686,38830,38831,38832,38833,38834,38835,38836,38837,38838,38839,38840,38841,38890,38891,38892,38893,38894,38895,38896,38897,38898,38899,38900,38901,38902,38903,38904,38905,38906,38907,38908,38909,38910,38911,38912,38913,38914,38915,38916,38917,38918,38919,38920,38921,38922,38923,38924,38925,38926,38927,38928,38929,38930,38931,38932,38933,38934,38935,38936,38937,38938,38939,38953,38954,38955,38956,39236,39239,39241,39242,39243,39244,39308,39309,39310,39311,39312,39313,39314,39315,39316,39317,39318,39319,39320,39321,39322,39323,39324,39379,39380,39381,39382,39384,39385,39386,39387,39390,39392,39393,39394,39395,39401,39402,39403,39404,39405,39406,39407,39408,39409,39410,39411,39412,39413,39414,39415,39416,39419,39421,39422,39423,39424,39425,39427,39428,39429,39430,39431,39460,39461,39462,39463,39464,39465,39466,39467,39468,39469,39470,39474,39476,39478,39482,39484,39490,39491,39493,39494,39501,39502,39549,39550,39551,39552,39553,39554,39555,39556,39557,39558,39559,39560,39561,39562,39563,39564,39565,39566,39567,39582,39583,39585,39586,39587,39588,39589,39590,39591,39592,39595,39596,39598,39600,39601,39602,39603,39604,39605,39606,39607,39608,39609,39610,39616,39617,39618]";
}