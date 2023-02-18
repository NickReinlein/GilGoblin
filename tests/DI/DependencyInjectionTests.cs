using System.Reflection;
using GilGoblin.Crafting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using System.Net;
using GilGoblin.Crafting;
using GilGoblin.DI;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

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

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();

        _configuration = new ConfigurationBuilder()
        //.AddApplicationConfigurationsSource<Startup>("tests")
        .Build();

        _environment = Substitute.For<IWebHostEnvironment>();
        _environment.EnvironmentName.Returns("production");
    }

    [Test]
    public void GivenAHostBuilder_WhenWeSetupHost_ThenWeSucceed()
    {
        var hostBuilder = Bootstrap.CreateHostBuilder().Build();
        Assert.That(hostBuilder, Is.Not.Null);
    }

    [Test]
    public void GivenAGilGoblin_WhenWeRun_ThenWeSucceed()
    {
        var program = new Program();
        Assert.That(program is not null);
    }
}
