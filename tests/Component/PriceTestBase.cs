using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class PriceTestBase : TestBase
{
    private const string priceEndpoint = "price/";

    [TestCaseSource(nameof(PriceTestCases))]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAPrice(
        int worldId,
        int itemId,
        bool isHq)
    {
        var fullEndpoint = $"price/{worldId}/{itemId}/{isHq}";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var price = await response.Content.ReadFromJsonAsync<PricePoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(price, Is.Not.Null);
            Assert.That(price!.ItemId, Is.EqualTo(itemId));
            Assert.That(price.WorldId, Is.EqualTo(worldId));
            Assert.That(price.IsHq, Is.EqualTo(isHq));
            Assert.That(price.Updated - DateTimeOffset.UtcNow, Is.LessThanOrEqualTo(TimeSpan.FromMinutes(5)));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_TheNoContentIsReturned()
    {
        const string fullEndpoint = $"{priceEndpoint}/103484654/false";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.IsSuccessStatusCode, Is.False);
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenACallToGetAll_WhenTheInputIsValid_ThenWeReceiveAllValidPrices(int worldId)
    {
        var fullEndpoint = $"{priceEndpoint}{worldId}/";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var prices = (await response.Content.ReadFromJsonAsync<IEnumerable<PricePoco>>(
            GetSerializerOptions()) ?? []).ToList();
        Assert.Multiple(() =>
        {
            var priceCount = prices.Count;
            Assert.That(priceCount, Is.GreaterThan(2), "Not enough entries received");
            Assert.That(prices.All(p => p.ItemId > 0), "ItemId is invalid");
            Assert.That(prices.All(p => p.WorldId == worldId), "WorldId is invalid");
            Assert.That(prices.All(p => p.Updated - DateTimeOffset.UtcNow <= TimeSpan.FromMinutes(5)),
                "Prices are not fresh");
        });
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint = "price/99999";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static IEnumerable<TestCaseData> PriceTestCases
    {
        get
        {
            return from world in ValidWorldIds
                from item in ValidItemsIds
                from hq in new[] { false, true }
                select new TestCaseData(world, item, hq)
                    .SetName($"GetPrice_World{world}_Item{item}_Hq{hq}");
        }
    }
}