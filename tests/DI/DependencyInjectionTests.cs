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

namespace GilGoblin.Tests.DI;

public class DependencyInjectionTests
{
    private IConfiguration _configuration;
    private IWebHostEnvironment _environment;
    private IServiceCollection _services;

    private IEnumerable<Type>? Controllers =>
        Assembly
            .GetAssembly(typeof(Startup))
            ?.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

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
    public void WhenWeConfigureServices_ThenDependenciesAreRegisteredCorrectlyForControllers()
    {
        var startup = new Startup(_configuration, _environment);
        startup.ConfigureServices(_services);
        _services
            .AddTransient(typeof(ILogger<>), typeof(NullLogger<>))
            .AddTransient<ILoggerFactory>(_ => NullLoggerFactory.Instance);

        var provider = _services.BuildServiceProvider();

        var controllersDependencies = Controllers!
            .Where(c => !c.IsAbstract)
            .SelectMany(c => c.GetConstructors().First().GetParameters());

        foreach (var dependency in controllersDependencies)
        {
            Assert.That(
                provider.GetService(dependency.ParameterType),
                Is.Not.Null,
                $"{dependency.ParameterType} is null"
            );
        }
    }

    [Test]
    public void GivenScopedDependencies_WhenWeResolveBackgroundService_ThenTheDependenciesAreResolved()
    {
        var startup = new Startup(_configuration, _environment);
        startup.ConfigureServices(_services);
        var provider = _services.BuildServiceProvider();

        var scopedService = provider.GetRequiredService<ICraftingCalculator>();

        Assert.That(scopedService, Is.Not.Null);
    }
}
