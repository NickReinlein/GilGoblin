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
    private const string itemEndpoint = "api/item/";

    [TestCaseSource(nameof(ValidItemsIds))]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAnItem(int itemId)
    {
        var fullEndpoint = $"{itemEndpoint}{itemId}";

        using var response = await _client.GetAsync(fullEndpoint);

        var item = await response.Content.ReadFromJsonAsync<ItemPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item!.Id, Is.EqualTo(itemId));
            Assert.That(item.IconId, Is.GreaterThan(0));
            Assert.That(item.Description, Is.Not.Null.And.Not.Empty);
            Assert.That(item.PriceLow, Is.GreaterThan(10));
            Assert.That(item.PriceMid, Is.GreaterThan(10));
            Assert.That(item.StackSize, Is.EqualTo(1));
            Assert.That(item.Level, Is.GreaterThanOrEqualTo(1));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint = $"{itemEndpoint}10348555";

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
            Assert.That(itemCount, Is.GreaterThanOrEqualTo(2), "Not enough entries received");
            Assert.That(items.All(p => p.Id > 0), "Id is invalid");
        });
    }
}