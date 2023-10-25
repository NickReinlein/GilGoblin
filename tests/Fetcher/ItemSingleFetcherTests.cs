using System.Net;
using System.Text.Json;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Fetcher;

public class ItemSingleFetcherTests : FetcherTests
{
    private ItemSingleFetcher _fetcher;
    private IItemRepository _repo;
    private IMarketableItemIdsFetcher _marketableIdsFetcher;
    private ILogger<ItemSingleFetcher> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        var pocos = GetMultipleDbPocos().ToList();
        var idList = pocos.Select(i => i.Id).ToList();
        _logger = Substitute.For<ILogger<ItemSingleFetcher>>();
        _repo = Substitute.For<IItemRepository>();
        _repo.GetAll().Returns(pocos);
        _marketableIdsFetcher = Substitute.For<IMarketableItemIdsFetcher>();
        _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns(idList);
        _fetcher = new ItemSingleFetcher(_repo, _marketableIdsFetcher, _logger, _client);
    }

    #region Fetcher calls

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
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
        GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        var returnedList = GetMultipleWebPocos().ToList();
        var idList = returnedList.Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.NotFound, ContentType, JsonSerializer.Serialize(returnedList));

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenNoIdsAreProvided_ThenWeReturnAnEmptyList()
    {
        var result = await _fetcher.FetchByIdsAsync(Array.Empty<int>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        var idList = GetMultipleWebPocos().Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(
                HttpStatusCode.OK,
                ContentType,
                JsonSerializer.Serialize("{}")
            );

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
    {
        var idList = GetMultipleWebPocos().Select(i => i.Id).ToList();
        _handler
            .When(GetUrl(idList[0]))
            .Respond(HttpStatusCode.OK, ContentType, "{ alksdfjs }");

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    #endregion

    #region Deserialization

    [Test]
    public void GivenWeDeserializeAResponse_WhenMultipleValidEntities_ThenWeDeserializeSuccessfully()
    {
        var result = JsonSerializer.Deserialize<ItemWebResponse>(
            GetItemJsonResponse1(),
            GetSerializerOptions()
        );

        var items = result?.GetContentAsList();

        Assert.Multiple(() =>
        {
            Assert.That(items, Is.Not.Null.And.Not.Empty);
            Assert.That(items!.Exists(item => item.Id == ItemId1));
            Assert.That(items.Exists(item => item.Id == ItemId2));
        });
    }

    [Test]
    public void GivenWeDeserializeAResponse_WhenAValidMarketableEntity_ThenWeDeserialize()
    {
        var result = JsonSerializer.Deserialize<ItemWebPoco>(
            GetItemJsonResponse1(),
            GetSerializerOptions()
        );

        Assert.That(result?.GetId() > 0);
    }

    #endregion

    private List<int> SetupResponse(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var pocoList = GetMultipleWebPocos().ToList();
        foreach (var poco in pocoList)
        {
            var id = poco.GetId();
            var url = GetUrl(id) + ItemSingleFetcher.ColumnsSuffix;
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

    private static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };

    private static int ItemId1 => 10972;
    private static int ItemId2 => 10973;

    private string GetUrl(int id) => $"https://xivapi.com/item/{id}";

    public static string GetItemJsonResponse1() =>
        """{"CanBeHq":1,"Description":"","ID":10972,"IconID":55724,"LevelItem":133,"Name":"Hardsilver Bangle of Fending","PriceLow":119,"PriceMid":20642,"StackSize":1}""";

    public static string GetItemJsonResponse2() =>
        """{"CanBeHq":1,"Description":"","ID":10973,"IconID":55732,"LevelItem":139,"Name":"Opal Bracelet of Fending","PriceLow":124,"PriceMid":21575,"StackSize":1}""";

    protected static List<ItemWebPoco> GetMultipleWebPocos()
    {
        var poco1 = new ItemWebPoco() { Id = ItemId1 };
        var poco2 = new ItemWebPoco { Id = ItemId2 };
        return new List<ItemWebPoco> { poco1, poco2 };
    }

    protected static List<ItemPoco> GetMultipleDbPocos()
    {
        var poco1 = new ItemPoco() { Id = ItemId1 };
        var poco2 = new ItemPoco { Id = ItemId2 };
        return new List<ItemPoco> { poco1, poco2 };
    }
}