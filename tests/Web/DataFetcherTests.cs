using System.Net;
using System.Text.Json;
using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class DataFetcherTests
{
    private const string BasePath = "http://localhost:55448/";
    private HttpClient _client;
    private MockDataFetcher _fetcher;
    private MockHttpMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _handler = new MockHttpMessageHandler();
        _client = _handler.ToHttpClient();

        _fetcher = new MockDataFetcher(BasePath, _client);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{BasePath}{badPath}").Respond(HttpStatusCode.NotFound, _contentType, "{}");

        var result = await _fetcher.GetAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var jsonObject = JsonSerializer.Serialize(new Apple { Id = 23 });
        _handler.When($"{BasePath}{goodPath}").Respond(HttpStatusCode.OK, _contentType, jsonObject);

        var result = await _fetcher.GetAsync(goodPath);

        Assert.That(result.Id, Is.EqualTo(23));
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{BasePath}{badPath}").Respond(HttpStatusCode.NotFound, _contentType, "{}");

        var result = await _fetcher.GetMultipleAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var jsonObject = JsonSerializer.Serialize(new Orange { Size = 324 });
        _handler.When($"{BasePath}{goodPath}").Respond(HttpStatusCode.OK, _contentType, jsonObject);

        var result = await _fetcher.GetMultipleAsync(goodPath);

        Assert.That(result.Size, Is.EqualTo(324));
    }

    private static readonly string _contentType = "application/json";
}

public class MockDataFetcher : DataFetcher<Apple, Orange>
{
    public MockDataFetcher(string basePath, HttpClient client)
        : base(basePath, client) { }
}

public class Apple
{
    public int Id { get; set; }
}

public class Orange
{
    public int Size { get; set; }
}
