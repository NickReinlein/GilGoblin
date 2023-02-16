using System.Net;
using GilGoblin.Crafting;
using GilGoblin.DI;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

public class DependencyInjectionTests
{
    // [Test]
    // public async Task WhenWeBuild_ThenTheDependenciesAreResolved()
    // {
    //     await using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(
    //         builder =>
    //             builder.ConfigureServices(services =>
    //             {
    //                 services.AddControllers();
    //                 services.AddEndpointsApiExplorer();
    //                 services.AddGoblinServices();
    //             })
    //     );

    //     var handler = application.Services.GetService<ICraftingCalculator>();

    //     Assert.That(handler, Is.Not.Null);
    // }

    [TestCase("/item/")]
    [TestCase("/item/100")]
    [TestCase("/recipe/")]
    [TestCase("/recipe/100")]
    [TestCase("/price/34/")]
    [TestCase("/price/34/100")]
    [TestCase("/craft/34/")]
    [TestCase("/craft/34/100")]
    public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
    {
        await using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(
            builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddControllers();
                    services.AddEndpointsApiExplorer();
                    services.AddGoblinServices();
                })
        );

        var client = application.CreateClient();

        var response = await client.GetAsync(endpoint);

        Assert.That(response, Is.Not.Null);
        var acceptableResponseCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
        Assert.True(acceptableResponseCodes.Contains(response.StatusCode));
    }
}
