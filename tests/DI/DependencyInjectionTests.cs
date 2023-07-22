using System.Reflection;
using GilGoblin.Api;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
    [TestCase(typeof(IPriceDataFetcher))]
    [TestCase(typeof(GilGoblinDatabase))]
    [TestCase(typeof(DataFetcher<PriceWebPoco, PriceWebResponse>))]
    public void GivenAGoblinService_WhenWeSetup_TheServiceIsResolved(Type serviceType)
    {
        var provider = _services.BuildServiceProvider();

        var scopedDependencyService = provider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }

    [Test]
    public void GivenGoblinServices_WhenWeSetup_ThenTheyAreNotNull()
    {
        var goblinServices = GetGoblinServicesList(_services);

        Assert.That(goblinServices, Is.Not.Empty);
        Assert.That(goblinServices.All(i => i is not null), Is.True);
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
