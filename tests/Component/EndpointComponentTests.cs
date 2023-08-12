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
        var fullEndpoint = $"http://localhost:55448/recipe/100";

        using var response = await _client.GetAsync(fullEndpoint);

        var responseContent = await response.Content.ReadAsStringAsync();
        // var recipeResponse = await response.Content.ReadFromJsonAsync<RecipePoco?>(
        //     GetSerializerOptions()
        // );
        // Assert.Multiple(() =>
        // {
        //     Assert.That(recipeResponse, Is.TypeOf<RecipePoco>());
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
