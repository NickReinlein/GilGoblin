using GilGoblin.Api;
using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Hosting;
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

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();

        _configuration = new ConfigurationBuilder().Build();

        _environment = Substitute.For<IWebHostEnvironment>();
        _environment.EnvironmentName.Returns("production");

        var startup = new Startup(_configuration, _environment);
        startup.ConfigureServices(_services);
    }

    [TestCase(typeof(IItemRepository))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(ICraftRepository<CraftSummaryPoco>))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(ICraftingCalculator))]
    public void GivenGoblinServices_WhenWeSetup_ThenTheyAreResolved(Type serviceType)
    {
        var provider = _services.BuildServiceProvider();

        var scopedDependencyService = provider.GetRequiredService<IItemRepository>();

        Assert.That(scopedDependencyService, Is.Not.Null);
    }

    [Test]
    public void GivenGoblinServices_WhenWeSetup_ThenTheyAreNotNull()
    {
        var goblinServices = GetGoblinServicesList(_services);

        Assert.That(goblinServices, Is.Not.Empty);
        foreach (var service in goblinServices)
        {
            Assert.That(service, Is.Not.Null);
        }
    }

    private static List<ServiceDescriptor> GetGoblinServicesList(IServiceCollection services) =>
        services
            .Where(
                s =>
                    s.ServiceType.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? s.ImplementationType?.FullName?.ToLowerInvariant().Contains("goblin")
                    ?? false
            )
            .ToList();
}
