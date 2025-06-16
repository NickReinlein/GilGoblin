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
    private const string recipeEndpoint = "api/recipe/";

    [TestCaseSource(nameof(ValidRecipeIds))]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveARecipe(int recipeId)
    {
        var fullEndpoint = $"{recipeEndpoint}{recipeId}";

        using var response = await _client.GetAsync(fullEndpoint);

        var recipe = await response.Content.ReadFromJsonAsync<RecipePoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(recipe!.Id, Is.EqualTo(recipeId));
            Assert.That(recipe.CraftType, Is.GreaterThan(0));
            Assert.That(ValidItemsIds, Does.Contain(recipe.TargetItemId));
            Assert.That(recipe.ResultQuantity, Is.GreaterThanOrEqualTo(1));
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