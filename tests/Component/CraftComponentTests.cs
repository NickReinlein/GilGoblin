using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Api.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

[Timeout(20000)]
public class CraftComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsInvalid_ThenBadRequestIsReturned()
    {
        const string fullEndpoint = "http://localhost:55448/craft/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GivenACallGetBestCrafts_WhenTheInputIsValid_ThenMultipleCraftSummariesAreReturned()
    {
        const string fullEndpoint = "http://localhost:55448/craft/34";

        using var response = await _client.GetAsync(fullEndpoint);

        var craftsRaw = await response.Content.ReadFromJsonAsync<IEnumerable<CraftSummaryPoco>>(GetSerializerOptions());

        Assert.Multiple(() =>
        {
            var crafts = craftsRaw?.ToList();
            Assert.That(crafts, Is.Not.Null);
            Assert.That(crafts, Has.Count.GreaterThan(5));
            Assert.That(crafts!.All(r => r.WorldId == 34));
            Assert.That(crafts!.All(r => r.ItemId > 0));
            Assert.That(crafts!.All(r => r.ItemInfo?.IconId > 0));
            Assert.That(crafts!.All(r => r.ItemInfo?.StackSize > 0));
            Assert.That(crafts!.All(r => r.ItemInfo?.PriceMid >= 0));
            Assert.That(crafts!.All(r => r.ItemInfo?.PriceLow >= 0));
            Assert.That(crafts!.All(r => r.Recipe?.Id > 0));
            Assert.That(crafts!.All(r => r.Recipe?.TargetItemId > 0));
            Assert.That(crafts!.All(r => r.Recipe?.ResultQuantity > 0));
        });
    }

    [TestCase("/34/1614654", HttpStatusCode.NotFound)]
    [TestCase("/1614654/1614654", HttpStatusCode.NotFound)]
    [TestCase("/1614654/100", HttpStatusCode.NotFound)]
    public async Task GivenACallToGetACraft_WhenTheInputIsInvalid_ThenFailureStatusCodeIsReturned(string urlSuffix,
        HttpStatusCode expectedErrorCode)
    {
        var fullEndpoint = $"http://localhost:55448/craft{urlSuffix}";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(expectedErrorCode));
    }
}