using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ItemTestBase : TestBase
{
    private const string itemEndpoint = "item/";

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAnItem()
    {
        var fullEndpoint = $"{itemEndpoint}10348";
        const string expectedDescription =
            "Black-and-white floorboards and carpeting of the same design as those used to furnish the Manderville Gold Saucer.";

        using var response = await _client.GetAsync(fullEndpoint);

        var item = await response.Content.ReadFromJsonAsync<ItemPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item!.Id, Is.EqualTo(10348));
            Assert.That(item.CanHq, Is.False);
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
        var fullEndpoint = $"{itemEndpoint}10348555";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllItems_ThenWeReceiveValidItems()
    {
        using var response = await _client.GetAsync(itemEndpoint);

        var items = (await response.Content.ReadFromJsonAsync<IEnumerable<ItemPoco>>(
            GetSerializerOptions()
        ))!.ToList();
        var itemCount = items.Count;
        Assert.Multiple(() =>
        {
            Assert.That(itemCount, Is.GreaterThan(2), "Not enough entries received");
            Assert.That(items.All(p => p.Id > 0), "Id is invalid");
        });
    }
}