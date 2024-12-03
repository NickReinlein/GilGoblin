using System;
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

        var result = await _fetcher.FetchByIdsAsync(idList);

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
        var idList = FilterOutNullIds(GetMultipleWebPocos());
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.NotFound, contentType, JsonSerializer.Serialize(idList));

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }


    [Test]
    public async Task GivenFetchByIdsAsync_WhenNoIdsAreProvided_ThenWeReturnAnEmptyList()
    {
        var result
            = await _fetcher.FetchByIdsAsync(Array.Empty<int>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        var idList = FilterOutNullIds(GetMultipleWebPocos());
        _handler
            .When(GetUrl(idList[0]))
            .Respond(
                HttpStatusCode.OK,
                contentType,
                JsonSerializer.Serialize("{}")
            );

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
    {
        var idList = FilterOutNullIds(GetMultipleWebPocos());
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.OK, contentType, "{ alksdfjs }");

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenFetchByIdsAsync_WhenFetchingData_ThenTheUrlIsDerivedFromEntities()
    {
        var idList = SetupResponse();

        var result = await _fetcher.FetchByIdsAsync(idList);

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
            .Respond(HttpStatusCode.OK, contentType, GetItemJsonResponse1());

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
                    .Respond(statusCode, contentType, GetItemJsonResponse1());
            else if (id == ItemId2)
                _handler
                    .When(url)
                    .Respond(statusCode, contentType, GetItemJsonResponse2());
        }

        return pocoList.Select(i => i.GetId()).ToList();
    }

    private static int ItemId1 => 10972;
    private static int ItemId2 => 10973;

    protected static List<ItemWebPoco> GetMultipleWebPocos()
    {
        var poco1 = new ItemWebPoco { Id = ItemId1 };
        var poco2 = new ItemWebPoco { Id = ItemId2 };
        return [poco1, poco2];
    }

    private static List<int> FilterOutNullIds(IEnumerable<ItemWebPoco> filterMe)
    {
        return filterMe.Where(w => w.GetId() > 0).Select(i => i.GetId()).ToList();
    }

    private string GetUrl(int id) => $"https://xivapi.com/item/{id}";

    public static string GetItemJsonResponse1() =>
        """
        {
        	"name": "Steel Longsword",
        	"description": null,
        	"iconId": 30435,
        	"level": 36,
        	"stackSize": 1,
        	"priceMid": 4795,
        	"priceLow": 41,
        	"canHq": true,
        	"id": 10972
        }
        """;

    public static string GetItemJsonResponse2() =>
        """
        {
        	"name": "Steel Axe",
        	"description": null,
        	"iconId": 30436,
        	"level": 36,
        	"stackSize": 1,
        	"priceMid": 5544,
        	"priceLow": 4354,
        	"canHq": true,
        	"id": 10973
        }
        """;

}