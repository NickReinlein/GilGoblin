using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
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
    public async Task GivenAFetchByIdsAsync_WhenReceivingAnEmptyList_ThenAnEmptyListIsReturned()
    {
        var result = await _fetcher.FetchByIdsAsync([]);

        Assert.That(result, Is.Empty);
    }
    
        
    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenAFetchByIdsAsync_WhenReceivingAnInvalidId_ThenAnEmptyListIsReturned(int id)
    {
        var result = await _fetcher.FetchByIdsAsync(new List<int> { id });

        Assert.That(result, Is.Empty);
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
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(appleId1));
            Assert.That(result[1].Id, Is.EqualTo(appleId2));
        });
    }
    
    [Test]
    public async Task GivenAFetchByIdsAsync_WhenReceivingMultipleEntries_ThenEachEntryIsFetched()
    {
        const int appleId1 = 23;
        const int appleId2 = 78;
        SetupValidResponse(appleId1, appleId2);
        var idList = new List<int> { appleId1, appleId2 };

        var result = await _fetcher.FetchByIdsAsync(idList);

        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(appleId1));
            Assert.That(result[1].Id, Is.EqualTo(appleId2));
        });
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
        _handler.When(expectedPath).Respond(HttpStatusCode.OK, contentType, jsonObject);
    }
}

public class MockBulkDataFetcher(
    string basePath,
    ILogger<BulkDataFetcher<Apple, AppleResponse>> logger,
    HttpClient client)
    : BulkDataFetcher<Apple, AppleResponse>(basePath, logger, client);

public class Apple(int id) : IIdentifiable

{
    public int Id { get; set; } = id;

    public int GetId() => Id;
}

public class AppleResponse(List<Apple> apples) : IResponseToList<Apple>

{
    public List<Apple> Apples { get; set; } = apples;

    public List<Apple> GetContentAsList() => Apples;
}