using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class CraftComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsValid_ThenWeReceiveACraftSummary()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34/1614";

        using var response = await _client.GetAsync(fullEndpoint);

        var craft = await response.Content.ReadFromJsonAsync<CraftSummaryPoco>(
            GetSerializerOptions()
        );
        Assert.Multiple(() =>
        {
            Assert.That(craft.WorldID, Is.EqualTo(34));
            Assert.That(craft.ItemID, Is.EqualTo(1614));
            Assert.That(craft.Name, Is.EqualTo("Iron Shortsword"));
            Assert.That(craft.CraftingCost, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.AverageSold, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.AverageListingPrice, Is.GreaterThan(100).And.LessThan(50000));
            Assert.That(craft.VendorPrice, Is.EqualTo(1297));
            Assert.That(craft.Recipe.TargetItemID, Is.EqualTo(1614));
            Assert.That(craft.Recipe.ID, Is.EqualTo(74));
        });
    }

    [Test]
    public async Task GivenACallToGetBestCraft_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallGetBestCrafts_WhenTheInputIsValid_ThenWeReceiveACraftSummary()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34";

        using var response = await _client.GetAsync(fullEndpoint);

        var crafts = await response.Content.ReadFromJsonAsync<IEnumerable<CraftSummaryPoco>>(
            GetSerializerOptions()
        );
        Assert.Multiple(() =>
        {
            Assert.That(crafts.Count(), Is.GreaterThan(5));
            Assert.That(crafts.All(r => r.WorldID == 34));
            Assert.That(crafts.All(r => r.ItemID > 0));
            Assert.That(crafts.All(r => r.AverageSold > 0));
            Assert.That(crafts.All(r => r.IconID > 0));
            Assert.That(crafts.All(r => r.StackSize > 0));
            Assert.That(crafts.All(r => r.VendorPrice > 0));
            Assert.That(crafts.All(r => r.Recipe.ID > 0));
            Assert.That(crafts.All(r => r.Recipe.TargetItemID > 0));
            Assert.That(crafts.All(r => r.Recipe.ResultQuantity > 0));
            Assert.That(crafts.All(r => r.Ingredients.Count() > 0));
        });
    }

    [Test]
    public async Task GivenACallToGetBestCrafts_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34/1614654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    // [Test]
    // public async Task GivenACallGetBestCrafts_WhenTheInputIsValid_ThenWeReceiveATimelyResponse()
    // {
    //     var fullEndpoint = $"http://localhost:55448/craft/34";
    //     _ = await _client.GetAsync(fullEndpoint);

    //     var timer = new Stopwatch();
    //     timer.Start();
    //     using var response = await _client.GetAsync(fullEndpoint);
    //     timer.Stop();

    //     // 1.2s 30
    //     // 9s 500
    //     // 466s all

    //     // now 6.9s 500
    //     // 78s all
    //     Assert.That(timer.Elapsed.TotalSeconds, Is.LessThan(10));
    // }
}
