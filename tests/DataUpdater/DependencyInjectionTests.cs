using System;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Converters;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Startup = GilGoblin.DataUpdater.Startup;

namespace GilGoblin.Tests.DataUpdater;

public class DataUpdaterDependencyInjectionTests
{
    protected WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Startup>();
    }

    [Test]
    public void GivenAGoblinDataUpdater_WhenWeStartup_ThenWeResolveWithoutError()
    {
        var client = _factory.CreateClient();

        Assert.That(client, Is.Not.Null);
    }

    [TestCase(typeof(IBulkDataFetcher<PriceWebPoco, PriceWebResponse>))]
    [TestCase(typeof(IPriceFetcher))]
    [TestCase(typeof(IWorldFetcher))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IMarketableItemIdsFetcher))]
    [TestCase(typeof(IPriceSaver))]
    [TestCase(typeof(IPriceConverter))]
    [TestCase(typeof(IRepositoryCache))]
    [TestCase(typeof(IItemCache))]
    [TestCase(typeof(IPriceCache))]
    [TestCase(typeof(IRecipeCache))]
    [TestCase(typeof(IItemRecipeCache))]
    [TestCase(typeof(IWorldCache))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(IPriceDataDetailConverter))]
    [TestCase(typeof(IPriceDataPointConverter))]
    [TestCase(typeof(IQualityPriceDataConverter))]
    [TestCase(typeof(IDailySaleVelocityConverter))]
    [TestCase(typeof(IDataSaver<DailySaleVelocityPoco>))]
    [TestCase(typeof(IDataSaver<WorldPoco>))]
    [TestCase(typeof(ICraftingCalculator))]
    public void GivenAGoblinDataUpdater_WhenWeStartup_ThenEachServiceIsResolvedSuccessfully(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}