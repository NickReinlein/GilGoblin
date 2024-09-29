// using System;
// using System.Threading.Tasks;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Database.Integration;
//
// [TestFixture]
// public class PriceSavingTests
// {
//     private GilGoblinDbContext _dbContext;
//
//     [SetUp]
//     public void SetUp()
//     {
//         var options = new DbContextOptionsBuilder<GilGoblinDbContext>().Options;
//         _dbContext = new GilGoblinDbContext(options, new ConfigurationRoot([]));
//     }
//
//     [Test]
//     public async Task SavePricePoco_ShouldSaveFullObject()
//     {
//         var pricePoco = new PricePoco(
//             1,
//             34,
//             true,
//             DateTime.UtcNow,
//             new MinListingPoco(
//                 1,
//                 34,
//                 false,
//                 DateTimeOffset.UtcNow,
//                 10,
//                 20,
//                 30,
//                 40
//             ),
//             RecentPurchase = new RecentPurchasePoco { Id = 1, Price = 90, Quantity = 5, Timestamp = DateTime.UtcNow },
//             AverageSalePrice = new AverageSalePricePoco
//             {
//                 Id = 1, AveragePrice = 95, Quantity = 8, Timestamp = DateTime.UtcNow
//             },
//             DailySaleVelocity = new DailySaleVelocityPoco { Id = 1, Velocity = 2, Timestamp = DateTime.UtcNow }
//     };
//
//     await _priceConverter.SavePricePocoAsync(pricePoco);
//
//     var savedPricePoco = await _dbContext.Prices.FindAsync(pricePoco.ItemId);
//
//     Assert.NotNull(savedPricePoco);
//     Assert.That(savedPricePoco.ItemId, Is.EqualTo(pricePoco.ItemId));
//     Assert.That(savedPricePoco.WorldId, Is.EqualTo(pricePoco.WorldId));
//     Assert.That(savedPricePoco.IsHq, Is.EqualTo(pricePoco.IsHq));
//     Assert.That(savedPricePoco.MinListing.Price, Is.EqualTo(pricePoco.MinListing.Price));
//     Assert.That(savedPricePoco.RecentPurchase.Price, Is.EqualTo(pricePoco.RecentPurchase.Price));
//     Assert.That(savedPricePoco.AverageSalePrice.AveragePrice, Is.EqualTo(pricePoco.AverageSalePrice.AveragePrice));
//     Assert.That(savedPricePoco.DailySaleVelocity.Velocity, Is.EqualTo(pricePoco.DailySaleVelocity.Velocity));
//     Assert.That(savedPricePoco.Updated, Is.EqualTo(pricePoco.Updated).Within(TimeSpan.FromSeconds(
//         1)); // Allows for slight timing discrepancies
// }
//
// [TearDown]
// public void TearDown()
// {
//     _dbContext.Database.EnsureDeleted();
//     _dbContext.Dispose();
// }
//
// }