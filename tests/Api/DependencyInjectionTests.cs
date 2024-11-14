using System;
using GilGoblin.Api;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.Api;

public class DependencyInjectionTests
{
    protected WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Startup>();
    }

    [Test]
    public void GivenAGoblinApi_WhenWeStartup_ThenWeResolveWithoutError()
    {
        var client = _factory.CreateClient();

        Assert.That(client, Is.Not.Null);
        Assert.That(client.BaseAddress, Is.Not.Null);
    }

    [TestCase(typeof(IItemRepository))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(IWorldRepository))]
    [TestCase(typeof(IRecipeCostRepository))]
    [TestCase(typeof(IRecipeProfitRepository))]
    [TestCase(typeof(ICraftRepository))]
    [TestCase(typeof(IPriceCache))]
    [TestCase(typeof(IRecipeCache))]
    [TestCase(typeof(ICalculatedMetricCache<RecipeCostPoco>))]
    [TestCase(typeof(ICalculatedMetricCache<RecipeProfitPoco>))]
    [TestCase(typeof(IItemCache))]
    [TestCase(typeof(IItemRecipeCache))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(ICraftingCalculator))]
    [TestCase(typeof(IRepositoryCache))]
    public void GivenAGoblinApi_WhenWeStartup_ThenEachServiceIsResolvedSuccessfully(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}