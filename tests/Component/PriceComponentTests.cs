using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class PriceComponentTests : ComponentTests
{
    private const int worldId = 21;
    private const int validItemId = 1604;
    private readonly string _priceEndpoint = $"{baseUrl}price/{worldId}";

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAPrice()
    {
        var fullEndpoint = $"{_priceEndpoint}/{validItemId}/false";
        
        using var response = await _client.GetAsync(fullEndpoint);

        var price = await response.Content.ReadFromJsonAsync<PricePoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(price, Is.Not.Null);
            Assert.That(price!.ItemId, Is.EqualTo(1604));
            Assert.That(price.WorldId, Is.EqualTo(21));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_TheNoContentIsReturned()
    {
        var fullEndpoint = $"{_priceEndpoint}/103484654/false";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsValid_ThenWeReceiveAllValidPrices()
    {
        var fullEndpoint = $"{_priceEndpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        var prices = (await response.Content.ReadFromJsonAsync<IEnumerable<PricePoco>>(
            GetSerializerOptions()) ?? []).ToList();

        Assert.Multiple(() =>
        {
            var priceCount = prices.Count;
            Assert.That(priceCount, Is.GreaterThan(1000), "Not enough entries received");
            Assert.That(prices.All(p => p.ItemId > 0), "ItemId is invalid");
            Assert.That(prices.All(p => p.WorldId == 21), "WorldId is incorrect");
        });
    }

    [Test]
    public async Task GivenACallToGetAll_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint =  $"{baseUrl}price/99999";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}