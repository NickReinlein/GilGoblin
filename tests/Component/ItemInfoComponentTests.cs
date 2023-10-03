using System.Net;
using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ItemInfoComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAnItemInfo()
    {
        const string fullEndpoint = "http://localhost:55448/item/10348";
        const string expectedDescription =
            "Black-and-white floorboards and carpeting of the same design as those used to furnish the Manderville Gold Saucer.";

        using var response = await _client.GetAsync(fullEndpoint);

        var item = await response.Content.ReadFromJsonAsync<ItemInfoPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(item.Id, Is.EqualTo(10348));
            Assert.That(item.CanBeHq, Is.False);
            Assert.That(item.IconId, Is.EqualTo(51024));
            Assert.That(item.Description, Is.EqualTo(expectedDescription));
            Assert.That(item.PriceLow, Is.GreaterThan(10));
            Assert.That(item.PriceMid, Is.GreaterThan(10));
            Assert.That(item.StackSize, Is.EqualTo(1));
            Assert.That(item.Level, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = $"http://localhost:55448/item/10348555";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllItemInfos_ThenWeReceiveValidItemInfos()
    {
        const string fullEndpoint = "http://localhost:55448/item/";

        using var response = await _client.GetAsync(fullEndpoint);

        var items = await response.Content.ReadFromJsonAsync<IEnumerable<ItemInfoPoco>>(
            GetSerializerOptions()
        );
        var itemCount = items.Count();
        var missingEntryThreshold = itemCount * MissingEntryPercentageThreshold;
        Assert.Multiple(() =>
        {
            Assert.That(itemCount, Is.GreaterThan(1000), "Not enough entries received");
            Assert.That(items.All(p => p.Id > 0), "Id is invalid");
            Assert.That(
                items.Count(p => p.IconId > 0),
                Is.GreaterThan(missingEntryThreshold),
                "IconId is invalid"
            );
            Assert.That(
                items.Count(p => p.Level > 0),
                Is.GreaterThan(missingEntryThreshold),
                "Level is invalid"
            );
            Assert.That(
                items.All(p => p.StackSize is >= 0 and <= 999999999),
                "StackSize is invalid"
            );
            // Only a few currencies have huge stacksize
            Assert.That(
                items.Count(p => p.StackSize is >= 999999999),
                Is.EqualTo(3),
                "StackSize is above 999999999 invalid"
            );
            Assert.That(
                items.Count(p => p.PriceLow > 0),
                Is.GreaterThan(missingEntryThreshold),
                "Missing PriceLow"
            );
            Assert.That(
                items.Count(p => p.PriceMid > 0),
                Is.GreaterThan(missingEntryThreshold),
                "Missing PriceMid"
            );
            Assert.That(
                items.Count(p => p.Description == string.Empty),
                Is.LessThan(missingEntryThreshold),
                "Missing a suspicious number of descriptions"
            );
            Assert.That(
                items.Count(p => p.CanBeHq),
                Is.GreaterThan(itemCount * (1.0f - MissingEntryPercentageThreshold)),
                "Missing a suspicious number of entries that can be High Quality"
            );
        });
    }
}