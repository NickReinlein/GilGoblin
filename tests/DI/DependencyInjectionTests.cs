using System.Net;
using System.Reflection;
using GilGoblin.Api;
using GilGoblin.Api.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

[NonParallelizable]
public class DependencyInjectionTests
{
    private WebApplicationBuilder _builder;

    [SetUp]
    public void SetUp()
    {
        _builder = Program.SetupBuilder(Array.Empty<string>());
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
    //     var builder = Program.SetupBuilder(Array.Empty<string>());
    //     builder.Configuration.por
    //     var application = builder.Build().AddAppGoblinServices();

    //     await application.RunAsync();

    //     var client = new HttpClient();
    //     var response = await client.GetAsync(endpoint);

    //     Assert.That(response, Is.Not.Null);
    //     var acceptableResponseCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
    //     Assert.True(acceptableResponseCodes.Contains(response.StatusCode));
    // }

    [Test]
    public void GivenABuilder_WhenWeSetupBuilder_ThenItIsNotNull()
    {
        Assert.That(_builder, Is.Not.Null);
        Assert.That(_builder.Services, Is.Not.Null);
    }

    [Test]
    public void GivenGoblinServices_WhenWeSetupServicesForBuilder_ThenTheyAreNotNull()
    {
        var goblinServices = _builder.Services
            .Where(
                s =>
                    s.ServiceType.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? s.ImplementationType?.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? false
            )
            .ToList();

        Assert.That(goblinServices, Is.Not.Empty);
        foreach (var service in goblinServices)
        {
            Assert.That(service, Is.Not.Null);
        }
    }
}
