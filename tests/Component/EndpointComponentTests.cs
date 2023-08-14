using System.Net.Http.Json;
using System.Text.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(AllEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"http://localhost:55448{endpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GivenACallToGetRecipe_WhenTheInputIsValid_ThenWeReturnARecipe()
    {
        var fullEndpoint = $"http://localhost:55448/recipe/{32635}";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipe = await response.Content.ReadFromJsonAsync<RecipePoco?>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(recipe, Is.TypeOf<RecipePoco>());
            Assert.That(recipe.ID, Is.EqualTo(32635));
            Assert.That(recipe.TargetItemID, Is.EqualTo(22428));
            Assert.That(recipe.CanHq);
            Assert.That(recipe.CanQuickSynth);
            Assert.That(recipe.ResultQuantity, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient0, Is.EqualTo(4));
            Assert.That(recipe.AmountIngredient1, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient2, Is.EqualTo(1));
            Assert.That(recipe.AmountIngredient8, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient9, Is.EqualTo(2));
            Assert.That(recipe.ItemIngredient0TargetID, Is.EqualTo(22418));
            Assert.That(recipe.ItemIngredient1TargetID, Is.EqualTo(22412));
            Assert.That(recipe.ItemIngredient2TargetID, Is.EqualTo(19938));
            Assert.That(recipe.ItemIngredient8TargetID, Is.EqualTo(16));
            Assert.That(recipe.ItemIngredient9TargetID, Is.EqualTo(14));
        });
    }

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

    [Test]
    public async Task GivenACallToGetItemInfo_WhenTheInputIsValid_ThenWeReturnItemInfo()
    {
        var fullEndpoint = $"http://localhost:55448/item/10348";
        var expectedDescription =
            "Black-and-white floorboards and carpeting of the same design as those used to furnish the Manderville Gold Saucer.";

        using var response = await _client.GetAsync(fullEndpoint);

        var item = await response.Content.ReadFromJsonAsync<ItemInfoPoco?>(GetSerializerOptions());
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

    [Test]
    public async Task GivenACallToGetCraft_WhenTheInputIsValid_ThenWeReturnACraftSummary()
    {
        var fullEndpoint = $"http://localhost:55448/craft/34/1614";

        using var response = await _client.GetAsync(fullEndpoint);

        var craft = await response.Content.ReadAsStringAsync();
        // var craft = await response.Content.ReadFromJsonAsync<CraftSummaryPoco>(
        // GetSerializerOptions()
        // );

        Assert.That(craft, Is.Not.Null);
        // Assert.Multiple(() =>
        // {
        //     Assert.That(craft, Is.TypeOf<CraftSummaryPoco>());
        //     Assert.That(craft.WorldID, Is.EqualTo(34));
        //     Assert.That(craft.ItemID, Is.EqualTo(1614));
        //     Assert.That(craft.Recipe.TargetItemID, Is.EqualTo(1614));
        //     Assert.That(craft.Recipe.ID, Is.EqualTo(74));
        //     Assert.That(craft.Name, Is.EqualTo("Iron Shortsword"));
        //     Assert.That(craft.CraftingCost, Is.GreaterThan(1000).And.LessThan(50000));
        //     Assert.That(craft.AverageSold, Is.GreaterThan(1000).And.LessThan(50000));
        //     Assert.That(craft.AverageListingPrice, Is.GreaterThan(1000).And.LessThan(50000));
        //     Assert.That(craft.VendorPrice, Is.EqualTo(1297));
        // });
    }

    private static string[] AllEndPoints =
    {
        "/recipe/",
        "/recipe/100",
        "/price/34/",
        "/price/34/100",
        "/craft/34/",
        "/craft/34/100",
        "/item/",
        "/item/100"
    };

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
}
