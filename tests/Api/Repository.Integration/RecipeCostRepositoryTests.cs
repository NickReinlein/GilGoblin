using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class RecipeCostRepositoryTests : GilGoblinDatabaseFixture
{
    private IRecipeCostCache _cache;
    private RecipeCostRepository _repo;

    [SetUp]
    public override async Task SetUp()
    {
        _cache = Substitute.For<IRecipeCostCache>();
        await base.SetUp();

        _repo = new RecipeCostRepository(_serviceProvider, _cache);
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenAGetAll_WhenEntriesExistForThatWorld_ThenTheRepositoryReturnsAllEntriesForThatWorld(int worldId)
    {
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeCost.Where(c => c.WorldId == worldId).ToList();

        var result = await _repo.GetAllAsync(worldId);

        Assert.That(result, Has.Count.GreaterThan(1));
        Assert.That(result, Is.EquivalentTo(expectedRecipes));
    }

    [Test]
    public async Task GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        var result = await _repo.GetAllAsync(999);

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(GetValidRecipeKeys))]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(
        (int id, int worldId, bool isHq) data)
    {
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeCost
            .FirstOrDefault(c =>
                c.WorldId == data.worldId &&
                c.RecipeId == data.id &&
                c.IsHq == data.isHq);

        var result = await _repo.GetAsync(data.worldId, data.id, data.isHq);

        Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!, Is.EqualTo(expectedRecipes));
            }
        );
    }

    [TestCaseSource(nameof(GetValidRecipeKeys))]
    public async Task GivenAGet_WhenTheIdIsValidButTheWorldIdIsNot_ThenNullIsReturned(
    (int id, int worldId, bool isHq) data)
    {
    
        var result = await _repo.GetAsync(9898, data.id, data.isHq);
    
        Assert.That(result, Is.Null);
    }
    
    // [TestCase(0)]
    // [TestCase(-1)]
    // [TestCase(9984545)]
    // public async Task GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    // {
    //     var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = await recipeCostRepo.GetAsync(WorldId, id);
    //
    //     Assert.That(result, Is.Null);
    // }
    //
    // [Test]
    // public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = recipeCostRepo.GetMultiple(WorldId, new[] { RecipeId, RecipeId2 });
    //
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.EqualTo(2));
    //         Assert.That(result.Any(p => p.RecipeId == RecipeId));
    //         Assert.That(result.Any(p => p.RecipeId == RecipeId2));
    //     });
    // }
    //
    // [Test]
    // public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = recipeCostRepo.GetMultiple(654687, new[] { RecipeId, RecipeId2 });
    //
    //     Assert.That(!result.Any());
    // }
    //
    // [Test]
    // public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = recipeCostRepo.GetMultiple(WorldId, new[] { RecipeId, 95419 });
    //
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.EqualTo(1));
    //         Assert.That(result.Any(p => p.RecipeId == RecipeId));
    //         Assert.That(!result.Any(p => p.WorldId != WorldId));
    //     });
    // }
    //
    // [Test]
    // public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = recipeCostRepo.GetMultiple(WorldId, new[] { 654645646, 9953121 });
    //
    //     Assert.That(!result.Any());
    // }
    //
    // [Test]
    // public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     var result = recipeCostRepo.GetMultiple(WorldId, Array.Empty<int>());
    //
    //     Assert.That(!result.Any());
    // }
    //
    // [Test]
    // public async Task GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     _cache.Get((WorldId, RecipeId)).Returns((RecipeCostPoco)null!);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     _ = await recipeCostRepo.GetAsync(WorldId, RecipeId);
    //
    //     _cache.Received(2).Get((WorldId, RecipeId));
    //     _cache
    //         .Received(1)
    //         .Add(
    //             (WorldId, RecipeId),
    //             Arg.Is<RecipeCostPoco>(
    //                 recipeCost => recipeCost.WorldId == WorldId && recipeCost.RecipeId == RecipeId
    //             )
    //         );
    // }
    //
    // [Test]
    // public async Task GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCost = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
    //     _cache.Get((WorldId, RecipeId)).Returns(recipeCost);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     _ = await recipeCostRepo.GetAsync(WorldId, RecipeId);
    //
    //     _cache.Received(1).Get((WorldId, RecipeId));
    //     _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    // }
    //
    // [Test]
    // public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //     var allRecipeCosts = context.RecipeCost.ToList();
    //
    //     await recipeCostRepo.FillCache();
    //
    //     allRecipeCosts.ForEach(
    //         recipe =>
    //             _cache
    //                 .Received(1)
    //                 .Add((recipe.WorldId, recipe.RecipeId), Arg.Any<RecipeCostPoco>())
    //     );
    // }
    //
    // [Test]
    // public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     context.RecipeCost.RemoveRange(context.RecipeCost);
    //     context.SaveChanges();
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     await recipeCostRepo.FillCache();
    //
    //     _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    // }
    //
    // [Test]
    // public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    // {
    //     var poco = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     _cache.Get((WorldId, RecipeId)).Returns(poco);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     await recipeCostRepo.AddAsync(poco);
    //
    //     _cache.Received(1).Get((WorldId, RecipeId));
    //     _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    // }
    //
    // [Test]
    // public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    // {
    //     var poco = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     _cache.Get((WorldId, RecipeId)).Returns((RecipeCostPoco)null!);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //
    //     await recipeCostRepo.AddAsync(poco);
    //
    //     _cache.Received(1).Get((WorldId, RecipeId));
    //     _cache.Received(1).Add((WorldId, RecipeId), poco);
    // }
    //
    // [Test]
    // public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    // {
    //     await using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeCostRepo = new RecipeCostRepository(context, _cache);
    //     var poco = new RecipeCostPoco { WorldId = 77, RecipeId = 999 };
    //     await recipeCostRepo.AddAsync(poco);
    //
    //     var result = await recipeCostRepo.GetAsync(77, 999);
    //
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.WorldId, Is.EqualTo(77));
    //         Assert.That(result.RecipeId, Is.EqualTo(999));
    //     });
    // }

    private static IEnumerable<(int, int, bool)> GetValidRecipeKeys()
    {
        return from recipeId in ValidRecipeIds
            from worldId in ValidWorldIds
            from hq in new[] { true, false }
            select (recipeId, worldId, hq);
    }
}