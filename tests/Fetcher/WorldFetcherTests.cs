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

public class WorldFetcherTests : FetcherTests
{
    private WorldFetcher _fetcher;
    private ILogger<WorldFetcher> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = Substitute.For<ILogger<WorldFetcher>>();
        _fetcher = new WorldFetcher(_logger, _client);
    }

    [Test]
    public async Task GivenGetAll_WhenSuccessfulAndTheResponseIsValid_ThenWeDeserializeSuccessfully()
    {
        SetupResponse();

        var result = await _fetcher.GetAllAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(e => e.Id == result[0].Id), Is.True);
            Assert.That(result.Any(e => e.Id == result[1].Id), Is.True);
        });
    }

    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.Moved)]
    [TestCase(HttpStatusCode.BadGateway)]
    [TestCase(HttpStatusCode.BadRequest)]
    public async Task GivenGetAll_WhenUnsuccessfulStatusCodeIsReturned_ThenWeReturnAnEmptyList(
        HttpStatusCode statusCode)
    {
        SetupResponse(statusCode);

        var result = await _fetcher.GetAllAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    [Test]
    public async Task GivenGetAll_WhenTheResponseIsInvalid_ThenWeReturnAnEmptyList()
    {
        SetupResponse(HttpStatusCode.OK, "blah blah");

        var result = await _fetcher.GetAllAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    [Test]
    public async Task GivenGetAll_WhenGivenARealJsonExample_ThenWeDeserializeSuccessfully()
    {
        SetupResponse(HttpStatusCode.OK, GetWorldsJsonResponse);

        var result = await _fetcher.GetAllAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(9));
            foreach (var world in result)
            {
                Assert.That(world.Id, Is.GreaterThan(0));
                Assert.That(world.Name, Is.Not.Null.Or.Empty);
            }
        });
    }

    private void SetupResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string? responseContent = null)
    {
        responseContent ??= JsonSerializer.Serialize(GetMultipleNewPocos().ToList());

        _handler
            .When(FetchWorldsAsyncUrl)
            .Respond(statusCode, ContentType, responseContent);
    }

    protected static IEnumerable<WorldWebPoco> GetMultipleNewPocos()
    {
        var poco1 = new WorldWebPoco(1, "First");
        var poco2 = new WorldWebPoco(2, "Second");
        return new List<WorldWebPoco> { poco1, poco2 };
    }

    private static string FetchWorldsAsyncUrl =>
        "https://universalis.app/api/v2/worlds";

    private static string GetWorldsJsonResponse =>
        """[{"id":21,"name":"Ravana"},{"id":22,"name":"Bismarck"},{"id":23,"name":"Asura"},{"id":24,"name":"Belias"},{"id":28,"name":"Pandaemonium"},{"id":29,"name":"Shinryu"},{"id":30,"name":"Unicorn"},{"id":31,"name":"Yojimbo"},{"id":2080,"name":"펜리르"}]""";
}