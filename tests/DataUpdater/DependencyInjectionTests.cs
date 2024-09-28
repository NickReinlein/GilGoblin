using System;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Converters;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.DataUpdater;
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
    public void GivenAGoblinDataUpdater_WhenWeStartup_ThenEachServiceIsResolvedSuccessfully(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}