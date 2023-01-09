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

    [Test]
    public async Task GivenScopedDependencies_WhenWeResolveBackgroundService_ThenTheDependenciesAreResolved2()
    {
        await using var application = new WebApplicationFactory<Program>().WithWebHostBuilder(
            builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddControllers();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();
                    services.AddGoblinServices();
                })
        );

        var client = application.CreateClient();

        var response = await client.GetAsync("/item/1");

        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        // TODO  continue here
    }

    [Test]
    public void GivenScopedDependencies_WhenWeResolveBackgroundService_ThenTheDependenciesAreResolved()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddGoblinServices();

        var app = builder.Build();

        var scopedService = app.Services.GetRequiredService<ICraftingCalculator>();
        Assert.That(scopedService, Is.Not.Null);
    }
}
