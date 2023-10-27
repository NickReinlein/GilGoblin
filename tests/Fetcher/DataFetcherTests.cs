using System.Net;
using System.Text.Json;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace GilGoblin.Tests.Fetcher;

public class DataFetcherTests : FetcherTests
{
    private const string basePath = "http://localhost:55448/";
    private MockBulkDataFetcher _fetcher;
    private ILogger<BulkDataFetcher<Apple, AppleResponse>> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<BulkDataFetcher<Apple, AppleResponse>>>();
        _fetcher = new MockBulkDataFetcher(basePath, _logger, _client);
    }

    [Test]
    public async Task GivenAFetchByIdsAsync_WhenReceivingASingleValidEntry_ThenThatEntryIsReturned()
    {
        const int appleId1 = 23;
        SetupValidResponse(appleId1);
        var idList = new List<int> { appleId1 };

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(appleId1));
    }

    [Test]
    public async Task GivenAFetchByIdsAsync_WhenReceivingMultipleValidEntries_ThenThoseEntriesAreReturned()
    {
        const int appleId1 = 23;
        const int appleId2 = 78;
        SetupValidResponse(appleId1, appleId2);
        var idList = new List<int> { appleId1, appleId2 };

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
        var expectedPath = $"{basePath}{appleId1}";
        expectedPath += appleId2 is not null ? $",{appleId2}" : "";
        _handler.When(expectedPath).Respond(HttpStatusCode.OK, ContentType, jsonObject);
    }
}

public class MockBulkDataFetcher : BulkDataFetcher<Apple, AppleResponse>

{
    public MockBulkDataFetcher(
        string basePath,
        ILogger<BulkDataFetcher<Apple, AppleResponse>> logger,
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