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

    // [Test]
    // public async Task GivenAFetchAndSerializeDataAsync_WhenThePathIsInvalid_ThenNullIsReturned()
    // {
    //     var badPath = "/badPath";
    //     _handler.When($"{Ar").Respond(HttpStatusCode.NotFound, ContentType, "{
    //     }
    //     ");
    //
    //     var result = await _fetcher.FetchByIdsAsync(badPath);
    //
    //     Assert.That(result, Is.Null);
    // }

    [Test]
    public async Task GivenAFetchByIdsAsync_WhenReceivingASingleValidEntry_ThenThatEntryIsReturned()
    {
        const int appleId1 = 23;
        var idList = new List<int> { appleId1 };
        SetupValidResponse(appleId1);

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(appleId1));
    }

    [Test]
    public async Task GivenAFetchByIdsAsync_WhenReceivingMultipleValidEntries_ThenThoseEntriesAreReturned()
    {
        const int appleId1 = 23;
        const int appleId2 = 78;
        var idList = new List<int> { appleId1, appleId2 };
        SetupValidResponse(appleId1, appleId2);

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(appleId1));
        Assert.That(result[1].Id, Is.EqualTo(appleId2));
    }

    private void SetupValidResponse(int appleId1, int? appleId2 = null)
    {
        var appleList = new List<Apple> { new(appleId1) };
        if (appleId2 is not null)
            appleList.Add(new Apple(appleId2.Value));
        var responseObject = new AppleResponse(appleList);
        var jsonObject = JsonSerializer.Serialize(responseObject);
        _handler.When($"{_basePath}{appleId1},{appleId2}").Respond(HttpStatusCode.OK, ContentType, jsonObject);
    }
}

public class MockDataFetcher : DataFetcher<Apple, AppleResponse>

{
public MockDataFetcher(
    string basePath,
    ILogger<DataFetcher<Apple, AppleResponse>> logger,
    HttpClient client)
    : base(basePath, logger, client)
{
}
}

public class Apple : IIdentifiable

{
public int Id { get; set; }

public Apple(int id)
{
    Id = id;
}

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