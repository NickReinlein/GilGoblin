using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Fetcher;

public class ItemFetcherTests : FetcherTests
{
    private ItemFetcher _fetcher;

    private ILogger<ItemFetcher> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<ItemFetcher>>();
        _fetcher = new ItemFetcher(_logger, _client);
    }

    #region Fetcher calls

    [Test]
    public async Task GivenFetchByIdsAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        var idList = SetupResponse();

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(idList.Count()));
            Assert.That(result.Any(r => r.Id == ItemId1));
            Assert.That(result.Any(r => r.Id == ItemId2));
        });
    }


    [Test]
    public async Task
        GivenFetchByIdsAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        var returnedList = GetMultipleWebPocos().ToList();
        var idList = returnedList.Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.NotFound, ContentType, JsonSerializer.Serialize(returnedList));

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenNoIdsAreProvided_ThenWeReturnAnEmptyList()
    {
        var result
            = await _fetcher.FetchByIdsAsync(CancellationToken.None, Array.Empty<int>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        var idList = GetMultipleWebPocos().Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(
                HttpStatusCode.OK,
                ContentType,
                JsonSerializer.Serialize("{}")
            );

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
    {
        var idList = GetMultipleWebPocos().Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.OK, ContentType, "{ alksdfjs }");

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenFetchingData_ThenTheUrlIsDerivedFromEntities()
    {
        var idList = SetupResponse();

        var result = await _fetcher.FetchByIdsAsync(CancellationToken.None, idList);

        foreach (var id in idList)
        {
            Assert.That(result.Exists(r => r.GetId() == id));
        }
    }

    #endregion

    #region Deserialization

    [Test]
    public void GivenWeDeserializeAResponse_WhenAValidMarketableEntity_ThenWeDeserialize()
    {
        var result = JsonSerializer.Deserialize<ItemWebPoco>(
            GetItemJsonResponse1(),
            ItemFetcher.GetSerializerOptions()
        );

        Assert.That(result?.GetId(), Is.GreaterThan(0));
    }

    [Test]
    public async Task GivenWeReadResponseContentAsync_WhenContentIsValid_ThenWeDeserialize()
    {
        _handler
            .When(GetUrl(ItemId1))
            .Respond(HttpStatusCode.OK, ContentType, GetItemJsonResponse1());

        var result = await _fetcher.FetchByIdAsync(ItemId1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetId(), Is.EqualTo(ItemId1));
    }

    #endregion

    private List<int> SetupResponse(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var pocoList = GetMultipleWebPocos().ToList();
        foreach (var poco in pocoList)
        {
            var id = poco.GetId();
            var url = GetUrl(id) + ItemFetcher.ColumnsSuffix;
            if (id == ItemId1)
                _handler
                    .When(url)
                    .Respond(statusCode, ContentType, GetItemJsonResponse1());
            else if (id == ItemId2)
                _handler
                    .When(url)
                    .Respond(statusCode, ContentType, GetItemJsonResponse2());
        }

        return pocoList.Select(i => i.GetId()).ToList();
    }

    private static int ItemId1 => 10972;
    private static int ItemId2 => 10973;

    private string GetUrl(int id) => $"https://xivapi.com/item/{id}";

    public static string GetItemJsonResponse1() =>
        """{"CanBeHq":1,"Description":"","ID":10972,"IconID":55724,"LevelItem":133,"Name":"Hardsilver Bangle of Fending","PriceLow":119,"PriceMid":20642,"StackSize":1}""";

    public static string GetItemJsonResponse2() =>
        """{"CanBeHq":1,"Description":"","ID":10973,"IconID":55732,"LevelItem":139,"Name":"Opal Bracelet of Fending","PriceLow":124,"PriceMid":21575,"StackSize":1}""";

    protected static List<ItemWebPoco> GetMultipleWebPocos()
    {
        var poco1 = new ItemWebPoco { Id = ItemId1 };
        var poco2 = new ItemWebPoco { Id = ItemId2 };
        return new List<ItemWebPoco> { poco1, poco2 };
    }
}