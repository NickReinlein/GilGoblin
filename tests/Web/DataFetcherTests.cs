using System.Net;
using System.Text.Json;
using GilGoblin.DataUpdater;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class DataFetcherTests : FetcherTests
{
    private readonly string _basePath = "http://localhost:55448/";
    private MockDataFetcher _fetcher;
    private ILogger<DataFetcher<Apple, AppleResponse>> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<DataFetcher<Apple, AppleResponse>>>();
        _fetcher = new MockDataFetcher(_basePath, _logger, _client);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{_basePath}{badPath}").Respond(HttpStatusCode.NotFound, ContentType, "{}");

        var result = await _fetcher.FetchAndSerializeDataAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var appleId = 23;
        var jsonObject = JsonSerializer.Serialize(new Apple { Id = appleId });
        _handler.When($"{_basePath}{goodPath}").Respond(HttpStatusCode.OK, ContentType, jsonObject);

        var result = await _fetcher.FetchAndSerializeDataAsync(goodPath);

        Assert.That(result?.GetContentAsList().First().Id, Is.EqualTo(appleId));
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{_basePath}{badPath}").Respond(HttpStatusCode.NotFound, ContentType, "{}");

        var result = await _fetcher.FetchAndSerializeDataAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var appleID = 324;
        var appleList = new List<Apple> { new() { Id = appleID } };
        var appleResponse = new AppleResponse(appleList);
        var jsonObject = JsonSerializer.Serialize(appleResponse);
        _handler.When($"{_basePath}{goodPath}").Respond(HttpStatusCode.OK, ContentType, jsonObject);

        var result = await _fetcher.FetchAndSerializeDataAsync(goodPath);

        var resultList = result?.GetContentAsList();
        Assert.That(resultList?.First().Id, Is.EqualTo(appleID));
    }
}

public class MockDataFetcher : DataFetcher<Apple, AppleResponse>
{
    public MockDataFetcher(string basePath, ILogger<DataFetcher<Apple, AppleResponse>> logger, HttpClient client)
        : base(basePath, logger, client)
    {
    }
}

public class Apple : IIdentifiable
{
    public int Id { get; set; }
    public int GetId() => Id;
}

public class AppleResponse : IResponseToList<Apple>
{
    public List<Apple> Apples { get; set; }

    public AppleResponse(List<Apple> apples)
    {
        Apples = apples;
    }

    public List<Apple> GetContentAsList() => Apples;
}