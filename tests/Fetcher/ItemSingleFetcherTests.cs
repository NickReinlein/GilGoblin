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

    #region Basic

    [Test]
    public void GivenWeCreateAItemFetcher_WhenTheClientIsNotProvided_ThenWeCanUseADefaultClientInstead()
    {
        var defaultItems = _fetcher.ItemsPerPage;

        _fetcher = new ItemSingleFetcher(_repo, _marketableIdsFetcher, _logger);

        Assert.That(_fetcher.ItemsPerPage, Is.GreaterThanOrEqualTo(defaultItems));
    }

    #endregion

    #region Fetcher calls

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        var idList = SetupResponse();

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(r => r.Id == ItemId1));
            Assert.That(result.Any(r => r.Id == ItemId2));
        });
    }


    [Test]
    public async Task
        GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsStatusCodeIsUnsuccessful_ThenWeReturnAnEmptyList()
    {
        var returnedList = GetMultipleWebPocos().ToList();
        var idList = returnedList.Select(i => i.Id);
        _handler
            .When(GetUrl)
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
            .When(GetUrl)
            .Respond(
                HttpStatusCode.OK,
                ContentType,
                JsonSerializer.Serialize(GetMultipleWebPocos())
            );

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallFetchMultipleItemsAsync_WhenTheResponseIsNull_ThenWeReturnAnEmptyList()
    {
        var idList = GetMultipleWebPocos().Select(i => i.Id).ToList();
        _handler
            .When(GetUrl)
            .Respond(HttpStatusCode.OK, ContentType, "{ alksdfjs }");

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Empty);
    }


    [Test]
    public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsEmpty_ThenWeReturnAnEmptyResult()
    {
        _handler.When(Arg.Any<string>()).Respond(HttpStatusCode.OK, ContentType, "{}");

        var result = await _fetcher.GetIdsAsBatchJobsAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsValid_ThenWeReturnBatchesOfIds()
    {
        _handler
            .When(Arg.Any<string>())
            .Respond(
                HttpStatusCode.OK,
                ContentType,
                JsonSerializer.Serialize(new List<int> { 1, 2, 3 })
            );

        var result = await _fetcher.GetIdsAsBatchJobsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Count, Is.EqualTo(3));
        });
    }

    [TestCase(1, 1)]
    [TestCase(2, 1)]
    [TestCase(3, 1)]
    [TestCase(4, 2)]
    [TestCase(10, 4)]
    public async Task GivenWeCallGetAllIdsAsBatchJobsAsync_WhenTheIDListReturnedIsValid_ThenWeReturnIdsBatchedCorrectly(
        int entryCount,
        int expectedPages
    )
    {
        _fetcher.ItemsPerPage = 3;
        var idList = Enumerable.Range(1, entryCount);
        _handler
            .When(Arg.Any<string>())
            .Respond(HttpStatusCode.OK, ContentType, JsonSerializer.Serialize(idList));

        var result = await _fetcher.GetIdsAsBatchJobsAsync();

        Assert.That(result, Has.Count.EqualTo(expectedPages));
    }

    #endregion

    #region Deserialization

    [Test]
    public void GivenWeDeserializeAResponse_WhenMultipleValidEntities_ThenWeDeserializeSuccessfully()
    {
        var result = JsonSerializer.Deserialize<ItemWebResponse>(
            GetItemJsonResponse(),
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
            GetItemJsonResponse(),
            GetSerializerOptions()
        );

        Assert.That(result?.GetId() > 0);
    }

    #endregion

    private IEnumerable<int> SetupResponse(
        bool success = true,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string? expectedUrl = null)
    {
        var pocoList = GetMultipleWebPocos().ToList();
        var idList = pocoList.Select(i => i.Id).ToList();
        var dict = pocoList.ToDictionary(l => l.Id);
        var webResponse = new ItemWebResponse(dict);
        var responseContent = success ? JsonSerializer.Serialize(webResponse) : JsonSerializer.Serialize(idList);
        var url = expectedUrl ?? GetUrl;

        _handler
            .When(url)
            .Respond(statusCode, ContentType, responseContent);
        return idList;
    }

    private static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };

    private static int ItemId1 => 10972;
    private static int ItemId2 => 10973;

    private string GetUrl => $"https://xivapi.com/item/{ItemId1}";

    public static string GetItemJsonResponse() =>
        """{"CanBeHq":1,"Description":"","ID":10972,"IconID":55724,"LevelItem":133,"Name":"Hardsilver Bangle of Fending","PriceLow":119,"PriceMid":20642,"StackSize":1}""";

    protected static List<ItemWebPoco> GetMultipleWebPocos()
    {
        var poco1 = new ItemWebPoco() { Id = _itemId1 };
        var poco2 = new ItemWebPoco { Id = _itemId2 };
        return new List<ItemWebPoco> { poco1, poco2 };
    }

    protected static List<ItemPoco> GetMultipleDbPocos()
    {
        var poco1 = new ItemPoco() { Id = _itemId1 };
        var poco2 = new ItemPoco { Id = _itemId2 };
        return new List<ItemPoco> { poco1, poco2 };
    }

    private static readonly int _itemId1 = 65711;
    private static readonly int _itemId2 = 86984;
}