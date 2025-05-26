using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class RecipeTestBase : TestBase
{
    private const string recipeEndpoint = "recipe/";

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveARecipe()
    {
        var fullEndpoint = $"{recipeEndpoint}32635";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipe = await response.Content.ReadFromJsonAsync<RecipePoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(recipe!.Id, Is.EqualTo(32635));
            Assert.That(recipe.CraftType, Is.EqualTo(3));
            Assert.That(recipe.TargetItemId, Is.EqualTo(22428));
            Assert.That(recipe.CanHq);
            Assert.That(recipe.CanQuickSynth);
            Assert.That(recipe.ResultQuantity, Is.EqualTo(2));
            Assert.That(recipe.ItemIngredient0TargetId, Is.EqualTo(22418));
            Assert.That(recipe.AmountIngredient0, Is.EqualTo(4));
            Assert.That(recipe.ItemIngredient1TargetId, Is.EqualTo(22412));
            Assert.That(recipe.AmountIngredient1, Is.EqualTo(2));
            Assert.That(recipe.ItemIngredient2TargetId, Is.EqualTo(19938));
            Assert.That(recipe.AmountIngredient2, Is.EqualTo(1));
            Assert.That(recipe.ItemIngredient8TargetId, Is.EqualTo(16));
            Assert.That(recipe.AmountIngredient8, Is.EqualTo(2));
            Assert.That(recipe.ItemIngredient9TargetId, Is.EqualTo(14));
            Assert.That(recipe.AmountIngredient9, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReceiveNoContent()
    {
        const string fullEndpoint = $"{recipeEndpoint}35454454";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllRecipes_ThenWeReceiveValidRecipes()
    {
        const string fullEndpoint = $"{recipeEndpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipes = (await response.Content.ReadFromJsonAsync<IEnumerable<RecipePoco>>(
            GetSerializerOptions()
        ) ?? []).ToList();

        var recipeCount = recipes.Count;
        Assert.Multiple(() =>
        {
            Assert.That(recipeCount, Is.GreaterThanOrEqualTo(2), "Not enough entries received");
            Assert.That(recipes.All(p => p.Id > 0), "ItemId is invalid");
        });
    }
}