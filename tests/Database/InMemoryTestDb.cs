// using System;
// using GilGoblin.Database.Pocos;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Database;
//
// public class InMemoryTestDb
// {
//     protected DbContextOptions<TestGilGoblinDbContext> _options;
//     protected IConfiguration _configuration;
//
//     [OneTimeSetUp]
//     public virtual void OneTimeSetUp()
//     {
//         _configuration = Substitute.For<IConfiguration>();
//
//         _options = Substitute.For<DbContextOptions<TestGilGoblinDbContext>>();
//         _options = new DbContextOptionsBuilder<TestGilGoblinDbContext>().Options;
//     }
//
//     [SetUp]
//     public virtual void SetUp()
//     {
//         CreateAllEntries();
//     }
//
//     [TearDown]
//     public virtual void TearDown()
//     {
//         DeleteAllEntries();
//     }
//
//     private void DeleteAllEntries()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         context.RecentPurchase.RemoveRange(context.RecentPurchase);
//         context.AverageSalePrice.RemoveRange(context.AverageSalePrice);
//         context.MinListing.RemoveRange(context.MinListing);
//         context.DailySaleVelocity.RemoveRange(context.DailySaleVelocity);
//         context.World.RemoveRange(context.World);
//         context.Item.RemoveRange(context.Item);
//         context.Recipe.RemoveRange(context.Recipe);
//         context.RecipeCost.RemoveRange(context.RecipeCost);
//         context.RecipeProfit.RemoveRange(context.RecipeProfit);
//         context.PriceDataPoints.RemoveRange(context.PriceDataPoints);
//         context.SaveChanges();
//     }
//
//     private void CreateAllEntries()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         context.RecipeCost.AddRange(
//             new RecipeCostPoco
//             {
//                 WorldId = 22,
//                 RecipeId = 11,
//                 RecentPurchaseCost = 107,
//                 AverageSalePriceCost = 97,
//                 MinListingPriceCost = 87,
//                 Updated = DateTimeOffset.UtcNow.AddHours(-1)
//             },
//             new RecipeCostPoco
//             {
//                 WorldId = 22,
//                 RecipeId = 11,
//                 RecentPurchaseCost = 107,
//                 AverageSalePriceCost = 97,
//                 MinListingPriceCost = 87,
//                 Updated = DateTimeOffset.UtcNow.AddHours(-1)
//             },
//             new RecipeCostPoco
//             {
//                 WorldId = 22,
//                 RecipeId = 12,
//                 RecentPurchaseCost = 297,
//                 AverageSalePriceCost = 277,
//                 MinListingPriceCost = 257,
//                 Updated = DateTimeOffset.UtcNow.AddHours(-1)
//             },
//             new RecipeCostPoco
//             {
//                 WorldId = 33,
//                 RecipeId = 88,
//                 RecentPurchaseCost = 224,
//                 AverageSalePriceCost = 204,
//                 MinListingPriceCost = 184,
//                 Updated = DateTimeOffset.UtcNow.AddHours(-1)
//             },
//             new RecipeCostPoco
//             {
//                 WorldId = 44,
//                 RecipeId = 99,
//                 RecentPurchaseCost = 351,
//                 AverageSalePriceCost = 331,
//                 MinListingPriceCost = 311,
//                 Updated = DateTimeOffset.UtcNow.AddHours(-1)
//             }
//         );
//         context.Recipe.AddRange(
//             new RecipePoco { Id = 11, TargetItemId = 5682, ItemIngredient0TargetId = 774, AmountIngredient0 = 3 },
//             new RecipePoco { Id = 12, TargetItemId = 9984, ItemIngredient0TargetId = 12, AmountIngredient0 = 5 },
//             new RecipePoco { Id = 13, TargetItemId = 111, ItemIngredient0TargetId = 14, AmountIngredient0 = 2 },
//             new RecipePoco { Id = 33, TargetItemId = 222, ItemIngredient0TargetId = 88, AmountIngredient0 = 7 },
//             new RecipePoco { Id = 44, TargetItemId = 333, ItemIngredient0TargetId = 99, AmountIngredient0 = 2 },
//             new RecipePoco { Id = 55, TargetItemId = 88, ItemIngredient0TargetId = 3, AmountIngredient0 = 3 },
//             new RecipePoco { Id = 88, TargetItemId = 88, ItemIngredient0TargetId = 1, AmountIngredient0 = 3 },
//             new RecipePoco { Id = 99, TargetItemId = 9984, ItemIngredient0TargetId = 2, AmountIngredient0 = 5 }
//         );
//         // context.Price.AddRange(
//         //     new PricePoco
//         //     {
//         //         WorldId = 22,
//         //         ItemId = 11,
//         //         AverageListingPrice = 11f,
//         //         AverageSold = 33f,
//         //         LastUploadTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
//         //     },
//         //     new PricePoco
//         //     {
//         //         WorldId = 22,
//         //         ItemId = 12,
//         //         AverageListingPrice = 33f,
//         //         AverageSold = 24f,
//         //         LastUploadTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
//         //     },
//         //     new PricePoco
//         //     {
//         //         WorldId = 33,
//         //         ItemId = 88,
//         //         AverageListingPrice = 65f,
//         //         AverageSold = 55f,
//         //         LastUploadTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
//         //     },
//         //     new PricePoco
//         //     {
//         //         WorldId = 44,
//         //         ItemId = 99,
//         //         AverageListingPrice = 87f,
//         //         AverageSold = 59f,
//         //         LastUploadTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
//         //     }
//         // );
//         context.Item.AddRange(
//             new ItemPoco { Id = 1, Name = "Item 1" },
//             new ItemPoco { Id = 2, Name = "Item 2" },
//             new ItemPoco { Id = 3, Name = "Item 3" },
//             new ItemPoco { Id = 22, Name = "Item 22" }
//         );
//         context.SaveChanges();
//     }
// }