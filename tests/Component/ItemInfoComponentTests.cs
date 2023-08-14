using System.Net.Http.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ItemInfoComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGetItemInfo_WhenTheInputIsValid_ThenWeReturnItemInfo()
    {
        var fullEndpoint = $"http://localhost:55448/item/10348";
        var expectedDescription =
            "Black-and-white floorboards and carpeting of the same design as those used to furnish the Manderville Gold Saucer.";

        using var response = await _client.GetAsync(fullEndpoint);

        var item = await response.Content.ReadFromJsonAsync<ItemInfoPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(item, Is.TypeOf<ItemInfoPoco>());
            Assert.That(item.ID, Is.EqualTo(10348));
            Assert.That(item.CanBeHq, Is.False);
            Assert.That(item.IconID, Is.EqualTo(51024));
            Assert.That(item.Description, Is.EqualTo(expectedDescription));
            Assert.That(item.VendorPrice, Is.EqualTo(15000));
            Assert.That(item.StackSize, Is.EqualTo(1));
            Assert.That(item.Level, Is.EqualTo(1));
        });
    }
}
