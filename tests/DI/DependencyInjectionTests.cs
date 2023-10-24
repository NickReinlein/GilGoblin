using GilGoblin.Cache;
using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Tests.Component;
using GilGoblin.Web;
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
    [TestCase(typeof(IBulkDataFetcher<PriceWebPoco>))]
    [TestCase(typeof(ISingleDataFetcher<ItemWebPoco>))]
    [TestCase(typeof(IPriceBulkDataFetcher))]
    [TestCase(typeof(IItemSingleFetcher))]
    [TestCase(typeof(IMarketableItemIdsFetcher))]
    [TestCase(typeof(IDataSaver<ItemWebPoco>))]
    // [TestCase(typeof(DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>))]
    public void GivenAGoblinService_WhenWeSetup_ThenTheServiceIsResolved(Type serviceType)
    {
        var scopedDependencyService = _services.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}