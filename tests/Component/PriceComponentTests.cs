using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class PriceComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAPrice()
    {
        const string fullEndpoint = "http://localhost:55448/price/34/10348";

        using var response = await _client.GetAsync(fullEndpoint);

        var price = await response.Content.ReadFromJsonAsync<PricePoco?>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(price, Is.Not.Null);
            Assert.That(price!.ItemId, Is.EqualTo(10348));
            Assert.That(price.WorldId, Is.EqualTo(34));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint = "http://localhost:55448/price/34/103484654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsValid_ThenWeReceiveAllValidPrices()
    {
        const string fullEndpoint = "http://localhost:55448/price/34";

        using var response = await _client.GetAsync(fullEndpoint);

        var prices = (await response.Content.ReadFromJsonAsync<IEnumerable<PricePoco>>(
            GetSerializerOptions()) ?? []).ToList();

        Assert.Multiple(() =>
        {
            var priceCount = prices.Count;
            Assert.That(priceCount, Is.GreaterThan(1000), "Not enough entries received");
            Assert.That(prices.All(p => p.ItemId > 0), "ItemId is invalid");
            Assert.That(prices.All(p => p.WorldId == 34), "WorldId is incorrect");
        });
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint = "http://localhost:55448/price/9999";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}