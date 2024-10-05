// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using GilGoblin.Api;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Tests.Database;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.InMemoryTest;
//
// public class InMemoryTestDb
// {
//     protected DbContextOptions<GilGoblinDbContext> _options;
//     protected IConfiguration _configuration;
//     protected IServiceProvider _serviceProvider;
//
//     protected static readonly List<int> ValidWorldIds = [34, 99];
//     protected static readonly List<int> ValidItemsIds = [134, 584, 654, 842];
//     protected static readonly List<int> ValidRecipeIds = [111, 122];
//
//     [OneTimeSetUp]
//     public virtual void OneTimeSetUp()
//     {
//         _configuration = Substitute.For<IConfiguration>();
//
//         _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _serviceProvider = new ServiceCollection()
//             .AddDbContext<GilGoblinDbContext>(options =>
//                 options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
//             .BuildServiceProvider();
//     }
//
//     [SetUp]
//     public virtual async Task SetUp()
//     {
//         await DeleteAllEntriesAsync();
//         await CreateAllEntriesAsync();
//     }
//
//     [TearDown]
//     public virtual async Task TearDown()
//     {
//         await DeleteAllEntriesAsync();
//     }
//
//     private async Task DeleteAllEntriesAsync()
//     {
//         await using var context = new GilGoblinDbContext(_options, _configuration);
//         context.RecentPurchase.RemoveRange(context.RecentPurchase);
//         context.AverageSalePrice.RemoveRange(context.AverageSalePrice);
//         context.MinListing.RemoveRange(context.MinListing);
//         context.DailySaleVelocity.RemoveRange(context.DailySaleVelocity);
//         context.World.RemoveRange(context.World);
//         context.Item.RemoveRange(context.Item);
//         context.Recipe.RemoveRange(context.Recipe);
//         context.RecipeCost.RemoveRange(context.RecipeCost);
//         context.RecipeProfit.RemoveRange(context.RecipeProfit);
//         context.Price.RemoveRange(context.Price);
//         context.PriceData.RemoveRange(context.PriceData);
//         context.WorldUploadTime.RemoveRange(context.WorldUploadTime);
//         await context.SaveChangesAsync();
//     }
//
//     private async Task CreateAllEntriesAsync()
//     {
//         await using var context = GetDbContext();
//
//         var recipeList = new RecipePoco[]
//             {
//                 new()
//                 {
//                     Id = ValidRecipeIds[0],
//                     TargetItemId = ValidItemsIds[0],
//                     ItemIngredient0TargetId = ValidItemsIds[1],
//                     AmountIngredient0 = 2,
//                     CanHq = true,
//                     CraftType = 3,
//                     CanQuickSynth = true,
//                     ResultQuantity = 1,
//                     RecipeLevelTable = 123
//                 },
//                 new()
//                 {
//                     Id = ValidRecipeIds[1],
//                     TargetItemId = ValidItemsIds[2],
//                     ItemIngredient0TargetId = ValidItemsIds[3],
//                     AmountIngredient0 = 3,
//                     CanHq = true,
//                     CraftType = 7,
//                     CanQuickSynth = true,
//                     ResultQuantity = 2,
//                     RecipeLevelTable = 123
//                 }
//             }
//             .ToList();
//         await context.Recipe.AddRangeAsync(recipeList);
//
//         var itemList = ValidItemsIds.Select(i => new ItemPoco
//         {
//             Name = $"Item {i}",
//             Description = $"Item {i} description",
//             IconId = i + 111,
//             CanHq = i % 2 == 0,
//             PriceLow = i * 3 + 11,
//             PriceMid = i * 3 + 12,
//             Level = i + 13,
//             StackSize = 1
//         }).ToList();
//         await context.Item.AddRangeAsync(itemList);
//
//         var validWorlds = ValidWorldIds
//             .Select(w => new WorldPoco { Id = w, Name = $"World {w}" })
//             .ToList();
//         await context.World.AddRangeAsync(validWorlds);
//
//
//         // var pricePoint = new PriceDataPointPoco(1, 11, true);
//         // context.Price.AddRange(
//         //     new PricePoco(ItemId: 1, WorldId: 34, IsHq: true, Updated: DateTimeOffset.UtcNow, null, null, null, null),
//         //     new PricePoco(ItemId: 2, WorldId: 34, IsHq: false, Updated: DateTimeOffset.UtcNow, null, null, null, null)
//         // );
//
//         await context.SaveChangesAsync();
//     }
//
//     protected GilGoblinDbContext GetDbContext()
//     {
//         return new GilGoblinDbContext(_options, _configuration);
//     }
// }