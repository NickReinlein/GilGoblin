// using System.Net.Http.Json;
// using System.Text.Json;
// using GilGoblin.Pocos;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Component;

// public class RecipeComponentTests : ComponentTests
// {
//     [Test]
//     public async Task GivenAGetAsync_WhenTheRecipeExists_ThenWeReturnTheRecipe()
//     {
//         var fullEndpoint = $"http://localhost:55448/recipe/100";

//         using var response = await _client.GetAsync(fullEndpoint);

//         var recipeResponse = await response.Content.ReadFromJsonAsync<RecipePoco?>(
//             GetSerializerOptions()
//         );
//         Assert.Multiple(() =>
//         {
//             Assert.That(response, Is.TypeOf<RecipePoco>());
//         });
//     }

//     protected static JsonSerializerOptions GetSerializerOptions() =>
//         new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
// }
