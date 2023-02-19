using GilGoblin.Api;
using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

public class DependencyInjectionTests
{
    private WebApplicationBuilder _builder;

    [SetUp]
    public void SetUp()
    {
        _builder = Program.SetupBuilder(Array.Empty<string>());
    }

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
}
