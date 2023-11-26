using System;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.DataUpdater;
using GilGoblin.Fetcher;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IMarketableItemIdsFetcher))]
    [TestCase(typeof(IItemFetcher))]
    [TestCase(typeof(ISingleDataFetcher<ItemWebPoco>))]
    [TestCase(typeof(IDataSaver<ItemPoco>))]
    [TestCase(typeof(IDataSaver<PricePoco>))]
    [TestCase(typeof(IDataUpdater<ItemPoco, ItemWebPoco>))]
    [TestCase(typeof(IDataUpdater<PricePoco, PriceWebPoco>))]
    public void GivenAGoblinDataUpdater_WhenWeStartup_ThenEachServiceIsResolvedSuccessfully(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}