using System.Net;
using System.Reflection;
using GilGoblin.Api;
using GilGoblin.Api.DI;
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
    private IConfiguration _configuration;
    private IWebHostEnvironment _environment;
    private IServiceCollection _services;

    private IEnumerable<Type>? Controllers =>
        Assembly
            .GetAssembly(typeof(Program))
            ?.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

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
    public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
    {
        await using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(
            builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddControllers();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                    services.AddGoblinServices();
                    services.AddGoblinDatabases();
                })
        );

        var client = application.CreateClient();

        var response = await client.GetAsync(endpoint);

        Assert.That(response, Is.Not.Null);
        var acceptableResponseCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
        Assert.True(acceptableResponseCodes.Contains(response.StatusCode));
    }

    [Test]
    public async Task GivenAHostBuilder_WhenWeSetupHost_ThenItIsNotNull()
    {
        var hostBuilder = Bootstrap.CreateHostBuilder(_environment.EnvironmentName).Build();
        Assert.That(hostBuilder, Is.Not.Null);
    }

    [Test]
    public async Task GivenAHostBuilder_WhenWeSetupHost_ThenWeSucceed()
    {
        var hostBuilder = Bootstrap.CreateHostBuilder(_environment.EnvironmentName).Build();

        Assert.That(hostBuilder, Is.Not.Null);
    }
}
