using GilGoblin.Cache;
using GilGoblin.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Tests.Component;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.DI;

public class DependencyInjectionTests : ComponentTests
{
    [TestCase(typeof(IItemRepository))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(ICraftRepository<CraftSummaryPoco>))]
    [TestCase(typeof(IPriceCache))]
    [TestCase(typeof(IRecipeCache))]
    [TestCase(typeof(IRecipeCostCache))]
    [TestCase(typeof(IItemCache))]
    [TestCase(typeof(IItemRecipeCache))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(ICraftingCalculator))]
    [TestCase(typeof(IRepositoryCache))]
    public void GivenAGoblinService_WhenWeSetup_ThenTheServiceIsResolved(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
    
    // [TestCase(typeof(IPriceFetcher))]
    // [TestCase(typeof(IItemFetcher))]
    // [TestCase(typeof(IMarketableItemIdsFetcher))]
    // [TestCase(typeof(IDataSaver<ItemPoco>))]
    // [TestCase(typeof(IDataSaver<PricePoco>))]
    // [TestCase(typeof(IDataUpdater<ItemPoco, ItemWebPoco>))]
    // [TestCase(typeof(IDataUpdater<PricePoco, PriceWebPoco>))]
}