using System.Net;
using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class CraftComponentTests : ComponentTests
{
    [Test, Timeout(10000), Ignore("Ignore for performance.. should still pass")]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsValid_ThenWeReceiveACraftSummary()
    {
        const string fullEndpoint = "http://localhost:55448/craft/34/1614";

        using var response = await _client.GetAsync(fullEndpoint);

        var craft = await response.Content.ReadFromJsonAsync<CraftSummaryPoco>(GetSerializerOptions());

        Assert.Multiple(() =>
        {
            Assert.That(craft.WorldId, Is.EqualTo(34));
            Assert.That(craft.ItemId, Is.EqualTo(1614));
            Assert.That(craft.Name, Is.EqualTo("Iron Shortsword"));
            Assert.That(craft.CraftingCost, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.AverageSold, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.AverageListingPrice, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.PriceMid, Is.GreaterThan(0));
            Assert.That(craft.PriceLow, Is.GreaterThan(0));
            Assert.That(craft.Recipe.TargetItemId, Is.EqualTo(1614));
            Assert.That(craft.Recipe.Id, Is.EqualTo(74));
        });
    }

    [Test]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = "http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, Timeout(20000), Ignore("Ignore for performance.. should still pass")]
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
            Assert.That(crafts.All(r => r.IconId > 0));
            Assert.That(crafts.All(r => r.StackSize > 0));
            Assert.That(crafts.All(r => r.PriceMid > 0));
            Assert.That(crafts.All(r => r.PriceLow > 0));
            Assert.That(crafts.All(r => r.Recipe.Id > 0));
            Assert.That(crafts.All(r => r.Recipe.TargetItemId > 0));
            Assert.That(crafts.All(r => r.Recipe.ResultQuantity > 0));
            Assert.That(crafts.All(r => r.Ingredients.Count() > 0));
        });
    }

    [Test]
    public async Task GivenACallToGetBestCrafts_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = "http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}