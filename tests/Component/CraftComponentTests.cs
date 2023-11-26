using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Api.Crafting;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class CraftComponentTests : ComponentTests
{

    [Test]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = "http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // [Test, Timeout(20000), Ignore("Ignore for performance.. should still pass")]
    [Test, Timeout(20000)]
    public async Task GivenACallGetBestCrafts_WhenTheInputIsValid_ThenWeReceiveACraftSummary()
    {
        var fullEndpoint = "http://localhost:55448/craft/34";

        using var response = await _client.GetAsync(fullEndpoint);

        var craftsRaw = await response.Content.ReadFromJsonAsync<IEnumerable<CraftSummaryPoco>>(GetSerializerOptions());
        var crafts = craftsRaw.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(crafts, Has.Count.GreaterThan(5));
            Assert.That(crafts.All(r => r.WorldId == 34));
            Assert.That(crafts.All(r => r.ItemId > 0));
            Assert.That(crafts.All(r => r.ItemInfo.IconId > 0));
            Assert.That(crafts.All(r => r.ItemInfo.StackSize > 0));
            Assert.That(crafts.All(r => r.ItemInfo.PriceMid >= 0));
            Assert.That(crafts.All(r => r.ItemInfo.PriceLow >= 0));
            Assert.That(crafts.All(r => r.Recipe.Id > 0));
            Assert.That(crafts.All(r => r.Recipe.TargetItemId > 0));
            Assert.That(crafts.All(r => r.Recipe.ResultQuantity > 0));
            Assert.That(crafts.All(r => r.Ingredients.Any()));
        });
    }

    [Test]
    public async Task GivenACallToGetBestCrafts_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = "http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}