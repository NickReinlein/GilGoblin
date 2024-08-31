// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using GilGoblin.Api.Cache;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Api.Repository;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Api.Repository;
//
// public class PriceRepositoryTests : PriceDependentTests
// {
//     private IPriceCache _cache;
//
//     [Test]
//     public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetAll(WorldId).ToList();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Has.Count.GreaterThanOrEqualTo(2));
//             Assert.That(result.Any(p => p.ItemId == RecipeId));
//             Assert.That(result.Any(p => p.ItemId == RecipeId2));
//             Assert.That(result.Any(p => p.WorldId != WorldId), Is.False);
//         });
//     }
//
//     [Test]
//     public void GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetAll(999);
//
//         Assert.That(!result.Any());
//     }
//
//     [TestCase(11)]
//     [TestCase(12)]
//     public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.Get(WorldId, id);
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Is.Not.Null);
//             Assert.That(result!.WorldId, Is.EqualTo(WorldId));
//             Assert.That(result.ItemId, Is.EqualTo(id));
//         });
//     }
//
//     [TestCase(11)]
//     [TestCase(12)]
//     public void GivenAGet_WhenTheIdIsValidButNotTheWorldId_ThenNullIsReturned(int id)
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.Get(854, id);
//
//         Assert.That(result, Is.Null);
//     }
//
//     [TestCase(0)]
//     [TestCase(-1)]
//     [TestCase(100)]
//     public void GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.Get(WorldId, id);
//
//         Assert.That(result, Is.Null);
//     }
//
//     [Test]
//     public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetMultiple(WorldId, new[] { RecipeId, RecipeId2 }).ToList();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Has.Count.GreaterThanOrEqualTo(2));
//             Assert.That(result.Any(p => p.ItemId == RecipeId));
//             Assert.That(result.Any(p => p.ItemId == RecipeId2));
//         });
//     }
//
//     [Test]
//     public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetMultiple(6845454, new[] { RecipeId, RecipeId2 });
//
//         Assert.That(!result.Any());
//     }
//
//     [Test]
//     public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetMultiple(WorldId, new[] { RecipeId, 99 }).ToList();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(result, Has.Count.EqualTo(1));
//             Assert.That(result.Any(p => p.ItemId == RecipeId));
//             Assert.That(result.All(p => p.WorldId == WorldId));
//         });
//     }
//
//     [Test]
//     public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetMultiple(WorldId, new[] { 654645646, 9953121 });
//
//         Assert.That(!result.Any());
//     }
//
//     [Test]
//     public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         var result = priceRepo.GetMultiple(WorldId, Array.Empty<int>());
//
//         Assert.That(!result.Any());
//     }
//
//     [Test]
//     public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         _ = priceRepo.Get(WorldId, ItemId);
//
//         _cache.Received(1).Get((WorldId, ItemId));
//         _cache
//             .Received(1)
//             .Add(
//                 (WorldId, ItemId),
//                 Arg.Is<PricePoco>(price => price.WorldId == WorldId && price.ItemId == ItemId)
//             );
//     }
//
//     [Test]
//     public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
//     {
//         using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var poco = new PricePoco { WorldId = WorldId, ItemId = ItemId };
//         _cache.Get((WorldId, ItemId)).Returns(null, poco);
//         var priceRepo = new PriceRepository(context, _cache);
//
//         _ = priceRepo.Get(WorldId, ItemId);
//         _ = priceRepo.Get(WorldId, ItemId);
//
//         _cache.Received(2).Get((WorldId, ItemId));
//         _cache
//             .Received(1)
//             .Add(
//                 (WorldId, ItemId),
//                 Arg.Is<PricePoco>(price => price.WorldId == WorldId && price.ItemId == ItemId)
//             );
//     }
//
//     [Test]
//     public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
//     {
//         await using var context = new TestGilGoblinDbContext(_options, _configuration);
//         var priceRepo = new PriceRepository(context, _cache);
//         var allPrices = context.Price.ToList();
//
//         await priceRepo.FillCache();
//
//         allPrices.ForEach(price => _cache.Received(1).Add((price.WorldId, price.ItemId), price));
//     }
//
//     [Test]
//     public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
//     {
//         await using var context = new TestGilGoblinDbContext(_options, _configuration);
//         context.Price.RemoveRange(context.Price);
//         await context.SaveChangesAsync();
//         var priceRepo = new PriceRepository(context, _cache);
//
//         await priceRepo.FillCache();
//
//         _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<PricePoco>());
//     }
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         _cache = Substitute.For<IPriceCache>();
//     }
// }