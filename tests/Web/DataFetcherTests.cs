using System.Net;
using System.Text.Json;
using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Web;

public class DataFetcherTests : FetcherTests
{
    private readonly string _basePath = "http://localhost:55448/";
    private MockDataFetcher _fetcher;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _fetcher = new MockDataFetcher(_basePath, _client);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{_basePath}{badPath}").Respond(HttpStatusCode.NotFound, ContentType, "{}");

        var result = await _fetcher.GetAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var appleId = 23;
        var jsonObject = JsonSerializer.Serialize(new Apple { Id = appleId });
        _handler.When($"{_basePath}{goodPath}").Respond(HttpStatusCode.OK, ContentType, jsonObject);

        var result = await _fetcher.GetAsync(goodPath);

        Assert.That(result.Id, Is.EqualTo(appleId));
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    {
        var badPath = "/badPath";
        _handler.When($"{_basePath}{badPath}").Respond(HttpStatusCode.NotFound, ContentType, "{}");

        var result = await _fetcher.GetMultipleAsync(badPath);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GivenAGetMultipleAsync_WhenThePathIsValid_ThenAMappedObjectIsReturned()
    {
        var goodPath = "/goodPath";
        var orangeSize = 324;
        var jsonObject = JsonSerializer.Serialize(new Orange { Size = orangeSize });
        _handler.When($"{_basePath}{goodPath}").Respond(HttpStatusCode.OK, ContentType, jsonObject);

        var result = await _fetcher.GetMultipleAsync(goodPath);

        Assert.That(result.Size, Is.EqualTo(orangeSize));
    }
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
