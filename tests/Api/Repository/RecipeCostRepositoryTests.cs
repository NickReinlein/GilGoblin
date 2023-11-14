using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class RecipeCostRepositoryTests : PriceDependentTests
{
    private IRecipeCostCache _costCache;

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(result.Any(p => p.RecipeId == RecipeId));
            Assert.That(result.Any(p => p.RecipeId == RecipeId2));
            Assert.That(!result.Any(p => p.WorldId != WorldId));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(WorldId, id);

        Assert.Multiple(() =>
        {
            Assert.That(result?.WorldId == WorldId);
            Assert.That(result.RecipeId == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValidButTheWorldIdIsNot_ThenNullIsReturned(int id)
    {
        var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(WorldId, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(WorldId, new[] { RecipeId, RecipeId2 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.RecipeId == RecipeId));
            Assert.That(result.Any(p => p.RecipeId == RecipeId2));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(654687, new[] { RecipeId, RecipeId2 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(WorldId, new[] { RecipeId, 95419 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.RecipeId == RecipeId));
            Assert.That(!result.Any(p => p.WorldId != WorldId));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(WorldId, new[] { 654645646, 9953121 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(WorldId, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((WorldId, RecipeId)).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        _ = await recipeCostRepo.GetAsync(WorldId, RecipeId);

        _costCache.Received(2).Get((WorldId, RecipeId));
        _costCache
            .Received(1)
            .Add(
                (WorldId, RecipeId),
                Arg.Is<RecipeCostPoco>(
                    recipeCost => recipeCost.WorldId == WorldId && recipeCost.RecipeId == RecipeId
                )
            );
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCost = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
        _costCache.Get((WorldId, RecipeId)).Returns(recipeCost);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        _ = await recipeCostRepo.GetAsync(WorldId, RecipeId);

        _costCache.Received(1).Get((WorldId, RecipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);
        var allRecipeCosts = context.RecipeCost.ToList();

        await recipeCostRepo.FillCache();

        allRecipeCosts.ForEach(
            recipe =>
                _costCache
                    .Received(1)
                    .Add((recipe.WorldId, recipe.RecipeId), Arg.Any<RecipeCostPoco>())
        );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.SaveChanges();
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.FillCache();

        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    {
        var poco = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((WorldId, RecipeId)).Returns(poco);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((WorldId, RecipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        var poco = new RecipeCostPoco { WorldId = WorldId, RecipeId = RecipeId };
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((WorldId, RecipeId)).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((WorldId, RecipeId));
        _costCache.Received(1).Add((WorldId, RecipeId), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);
        var poco = new RecipeCostPoco { WorldId = 77, RecipeId = 999 };
        await recipeCostRepo.Add(poco);

        var result = await recipeCostRepo.GetAsync(77, 999);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId, Is.EqualTo(77));
            Assert.That(result.RecipeId, Is.EqualTo(999));
        });
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _costCache = Substitute.For<IRecipeCostCache>();
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var oldDataTimestamp = DateTimeOffset.UtcNow.AddDays(-10);
        context.RecipeCost.Add(new RecipeCostPoco
        {
            WorldId = 34, RecipeId = RecipeId2, Cost = 107, Updated = oldDataTimestamp
        });
        context.SaveChanges();
    }
}