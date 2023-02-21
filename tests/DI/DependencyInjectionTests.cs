using System.Net;
using GilGoblin.Api.DI;
using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

[NonParallelizable]
public class DependencyInjectionTests
{
    private WebApplicationBuilder _builder;

    [SetUp]
    public void SetUp()
    {
        _builder = Array.Empty<string>().GetGoblinBuilder();
    }

    [Test]
    public void GivenABuilder_WhenWeSetup_ThenItIsNotNull()
    {
        Assert.That(_builder, Is.Not.Null);
        Assert.That(_builder.Services, Is.Not.Null);
    }

    [Test]
    public void GivenGoblinServices_WhenWeSetup_ThenTheyAreNotNull()
    {
        var goblinServices = GetGoblinServicesList();

        Assert.That(goblinServices, Is.Not.Empty);
        foreach (var service in goblinServices)
        {
            Assert.That(service, Is.Not.Null);
        }
    }

    [TestCase(typeof(IItemRepository))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(ICraftRepository<CraftSummaryPoco>))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(ICraftingCalculator))]
    public void GivenAGoblinService_WhenWeRunTheApi_ThenServiceIsAvailable(Type type)
    {
        var app = _builder.Build();

        var diCheck = app.Services.GetRequiredService(type);

        Assert.That(diCheck, Is.Not.Null);
    }

    // [TestCase("/item/")]
    // [TestCase("/item/100")]
    // [TestCase("/recipe/")]
    // [TestCase("/recipe/100")]
    // [TestCase("/price/34/")]
    // [TestCase("/price/34/100")]
    // [TestCase("/craft/34/")]
    // [TestCase("/craft/34/100")]
    // public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
    // {
    //     var app = _builder.Build();
    //     app.Urls.Add("http://localhost:9001");

    //     await app.RunAsync();

    //     var client = new HttpClient { Timeout = new TimeSpan(0, 0, 5) };
    //     var response = await client.GetAsync(endpoint);

    //     Assert.That(response, Is.Not.Null);
    //     var acceptableResponseCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
    //     Assert.True(acceptableResponseCodes.Contains(response.StatusCode));
    // }

    private List<ServiceDescriptor> GetGoblinServicesList() =>
        _builder.Services
            .Where(
                s =>
                    s.ServiceType.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? s.ImplementationType?.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? false
            )
            .ToList();
}
