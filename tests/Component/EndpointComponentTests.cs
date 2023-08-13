using System.Net.Http.Json;
using System.Text.Json;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(AllEndPoints))]
    public async Task GivenACallToGetAsync_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"http://localhost:55448{endpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GivenACallToRecipeGetAsync_WhenTheEndPointIsValid_ThenTheReturnsARecipe()
    {
        const int recipeId = 32635;
        var fullEndpoint = $"http://localhost:55448/recipe/{recipeId}";

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
