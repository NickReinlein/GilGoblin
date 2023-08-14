using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class CraftComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGetCraft_WhenTheInputIsValid_ThenWeReturnACraftSummary()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34/1614";

        using var response = await _client.GetAsync(fullEndpoint);

        var craft = await response.Content.ReadFromJsonAsync<CraftSummaryPoco>(
            GetSerializerOptions()
        );
        Assert.Multiple(() =>
        {
            Assert.That(craft, Is.Not.Null);
            Assert.That(craft, Is.TypeOf<CraftSummaryPoco>());
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
}
