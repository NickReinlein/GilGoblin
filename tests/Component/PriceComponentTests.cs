using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class PriceComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGetPrice_WhenTheInputIsValid_ThenWeReturnAPrice()
    {
        var fullEndpoint = $"http://localhost:55448/price/34/10348";

        using var response = await _client.GetAsync(fullEndpoint);

        var price = await response.Content.ReadFromJsonAsync<PricePoco?>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(price, Is.TypeOf<PricePoco>());
            Assert.That(price.ItemID, Is.EqualTo(10348));
            Assert.That(price.WorldID, Is.EqualTo(34));
            Assert.That(price.LastUploadTime, Is.GreaterThan(1674800561942));
            Assert.That(price.AverageListingPrice, Is.GreaterThan(200));
            Assert.That(price.AverageSold, Is.GreaterThan(200));
        });
    }
}
