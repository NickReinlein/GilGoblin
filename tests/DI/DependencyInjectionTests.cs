using System.Net;
using System.Reflection;
using GilGoblin.Crafting;
using GilGoblin.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

public class DependencyInjectionTests
{
    private IConfiguration _configuration;
    private IWebHostEnvironment _environment;
    private IServiceCollection _services;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();

        _configuration = new ConfigurationBuilder().Build();

        _environment = Substitute.For<IWebHostEnvironment>();
        _environment.EnvironmentName.Returns("production");
    }

    [TestCase("/item/")]
    [TestCase("/item/100")]
    [TestCase("/recipe/")]
    [TestCase("/recipe/100")]
    [TestCase("/price/34/")]
    [TestCase("/price/34/100")]
    [TestCase("/craft/34/")]
    [TestCase("/craft/34/100")]
    public async Task GivenScopedDependencies_WhenWeResolveBackgroundService_ThenTheDependenciesAreResolved(
        string endpoint
    )
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
