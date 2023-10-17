// using GilGoblin.Cache;
// using GilGoblin.Crafting;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using GilGoblin.Services;
// using GilGoblin.Tests.Component;
// using GilGoblin.Tests.Database;
// using GilGoblin.Web;
// using Microsoft.Extensions.DependencyInjection;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.DI;
//
// public class DependencyInjectionTests : ComponentTests
// {
//     // [TestCase(typeof(IItemRepository))]
//     // [TestCase(typeof(IRecipeRepository))]
//     // [TestCase(typeof(IPriceRepository<PricePoco>))]
//     // [TestCase(typeof(ICraftRepository<CraftSummaryPoco>))]
//     // [TestCase(typeof(IPriceCache))]
//     // [TestCase(typeof(IRecipeCache))]
//     // [TestCase(typeof(IRecipeCostCache))]
//     // [TestCase(typeof(IItemInfoCache))]
//     // [TestCase(typeof(IItemRecipeCache))]
//     // [TestCase(typeof(IRecipeGrocer))]
//     // [TestCase(typeof(ICraftingCalculator))]
//     // [TestCase(typeof(IRepositoryCache))]
//     // [TestCase(typeof(ISqlLiteDatabaseConnector))]
//     // [TestCase(typeof(IDatabaseLoader))]
//     // [TestCase(typeof(ICsvInteractor))]
//     // [TestCase(typeof(IBulkDataFetcher<PriceWebPoco>))]
//     // [TestCase(typeof(ISingleDataFetcher<ItemInfoWebPoco>))]
//     // [TestCase(typeof(IPriceBulkDataFetcher))]
//     // [TestCase(typeof(IItemInfoSingleFetcher))]
//     // [TestCase(typeof(IMarketableItemIdsFetcher))]
//     // [TestCase(typeof(IDataSaver<ItemInfoWebPoco>))]
//     // // [TestCase(typeof(DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>))]
//     // public void GivenAGoblinService_WhenWeSetup_ThenTheServiceIsResolved(Type serviceType)
//     // {
//     //     var serviceProvider = services.BuildServiceProvider();
//     //     using var context = serviceScope.ServiceProvider.GetRequiredService<serviceType>();
//     //     var provider = _services.BuildServiceProvider();
//     //
//     //     var scopedDependencyService = provider.GetRequiredService(serviceType);
//     //
//     //     Assert.That(scopedDependencyService, Is.Not.Null);
//     // }
//
//     [Test]
//     public void GivenGoblinServices_WhenWeSetup_ThenNoneAreNull()
//     {
//         var goblinServices = GetGoblinServicesList(_services);
//
//         Assert.That(goblinServices, Is.Not.Empty);
//         Assert.That(goblinServices.All(i => i is not null), Is.True);
//     }
//
//     private static List<ServiceDescriptor> GetGoblinServicesList(IServiceCollection services) =>
//         services
//             .Where(
//                 s =>
//                     s.ServiceType.FullName?.ToLowerInvariant().Contains("goblin")
//                     ?? s.ImplementationType?.FullName?.ToLowerInvariant().Contains("goblin")
//                     ?? false
//             )
//             .ToList();
// }