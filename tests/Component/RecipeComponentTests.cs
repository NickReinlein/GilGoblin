using System.Net;
using System.Net.Http.Json;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class RecipeComponentTests : ComponentTests
{
    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveARecipe()
    {
        var fullEndpoint = "http://localhost:55448/recipe/32635";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipe = await response.Content.ReadFromJsonAsync<RecipePoco?>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(recipe.Id, Is.EqualTo(32635));
            Assert.That(recipe.TargetItemId, Is.EqualTo(22428));
            Assert.That(recipe.CanHq);
            Assert.That(recipe.CanQuickSynth);
            Assert.That(recipe.ResultQuantity, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient0, Is.EqualTo(4));
            Assert.That(recipe.AmountIngredient1, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient2, Is.EqualTo(1));
            Assert.That(recipe.AmountIngredient8, Is.EqualTo(2));
            Assert.That(recipe.AmountIngredient9, Is.EqualTo(2));
            Assert.That(recipe.ItemIngredient0TargetId, Is.EqualTo(22418));
            Assert.That(recipe.ItemIngredient1TargetId, Is.EqualTo(22412));
            Assert.That(recipe.ItemIngredient2TargetId, Is.EqualTo(19938));
            Assert.That(recipe.ItemIngredient8TargetId, Is.EqualTo(16));
            Assert.That(recipe.ItemIngredient9TargetId, Is.EqualTo(14));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        var fullEndpoint = "http://localhost:55448/recipe/32655454";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllRecipes_ThenWeReceiveValidRecipes()
    {
        var fullEndpoint = "http://localhost:55448/recipe/";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipes = await response.Content.ReadFromJsonAsync<IEnumerable<RecipePoco>>(
            GetSerializerOptions()
        );

        var recipeList = recipes.ToList();
        var recipeCount = recipeList.Count();
        Assert.Multiple(() =>
        {
            Assert.That(recipeCount, Is.GreaterThan(1000), "Not enough entries received");
            Assert.That(recipeList.All(p => p.Id > 0), "ItemId is invalid");
            Assert.That(recipeList.All(p => p.ResultQuantity > 0), "ItemId is invalid");
            Assert.That(recipeList.All(p => p.TargetItemId > 0), "TargetItemId is invalid");
            Assert.That(recipeList.All(p => p.AmountIngredient0 > 0), "Missing AmountIngredient0");
            Assert.That(recipeList.All(p => p.ItemIngredient0TargetId > 0), "Missing ItemIngredient0TargetId");
            Assert.That(
                recipes.Count(p => p.AmountIngredient0 > 1),
                Is.GreaterThan(recipeCount * (1.0f - MissingEntryPercentageThreshold)),
                "Missing a suspicious number of entries with more than 1 ingredient for AmountIngredient0"
            );
            Assert.That(
                recipes.Count(p => (p.AmountIngredient9 + p.AmountIngredient1) > 1),
                Is.GreaterThan(recipeCount * (1.0f - MissingEntryPercentageThreshold)),
                "Missing a suspicious number of entries for multiple ingredients"
            );
        });
    }
}