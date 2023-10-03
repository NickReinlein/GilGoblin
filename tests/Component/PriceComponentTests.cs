using System.Net;
using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class PriceComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAPrice()
    {
        var fullEndpoint = $"http://localhost:55448/price/34/10348";

        using var response = await _client.GetAsync(fullEndpoint);

        var price = await response.Content.ReadFromJsonAsync<PricePoco?>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(price.ItemId, Is.EqualTo(10348));
            Assert.That(price.WorldId, Is.EqualTo(34));
            Assert.That(price.LastUploadTime, Is.GreaterThan(1674800561942));
            Assert.That(price.AverageListingPrice, Is.GreaterThan(200));
            Assert.That(price.AverageSold, Is.GreaterThan(200));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = $"http://localhost:55448/price/34/103484654";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsValid_ThenWeReceiveAllValidPrices()
    {
        var fullEndpoint = $"http://localhost:55448/price/34";

        using var response = await _client.GetAsync(fullEndpoint);

        var prices = await response.Content.ReadFromJsonAsync<IEnumerable<PricePoco>>(
            GetSerializerOptions()
        );
        var priceCount = prices.Count();
        Assert.Multiple(() =>
        {
            Assert.That(priceCount, Is.GreaterThan(1000), "Not enough entries received");
            Assert.That(prices.All(p => p.ItemId > 0), "ItemId is invalid");
            Assert.That(prices.All(p => p.WorldId == 34), "WorldId is incorrect");
            Assert.That(
                prices.All(p => p.LastUploadTime > 1574800561942),
                "LastUploadTime timestamp is invalid"
            );
            Assert.That(
                prices.Count(p => p.AverageListingPrice == 0),
                Is.LessThan(priceCount * MissingEntryPercentageThreshold),
                "Number of missing AverageListingPrice is suspiciously high"
            );
            Assert.That(
                prices.Count(p => p.AverageSold == 0),
                Is.LessThan(priceCount * MissingEntryPercentageThreshold),
                "Number of missing AverageSold is suspiciously high"
            );
        });
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = $"http://localhost:55448/price/9999";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
