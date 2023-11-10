using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class RecipeProfitRepositoryTests : InMemoryTestDb
{
    private IRecipeProfitCache _costCache;

    private static readonly int _worldId = 33;
    private static readonly int _recipeId = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetAll(22);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.RecipeId == 11));
            Assert.That(result.Any(p => p.RecipeId == 12));
            Assert.That(!result.Any(p => p.WorldId != 22));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = await recipeProfitRepo.GetAsync(22, id);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId == 22);
            Assert.That(result.RecipeId == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValidButNotTheWorldId_ThenNullIsReturned(int id)
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = await recipeProfitRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = await recipeProfitRepo.GetAsync(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetMultiple(22, new[] { 11, 12 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.RecipeId == 11));
            Assert.That(result.Any(p => p.RecipeId == 12));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetMultiple(_worldId, new[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetMultiple(22, new[] { 11, 99 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.RecipeId == 11));
            Assert.That(!result.Any(p => p.WorldId != 22));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetMultiple(22, new[] { _worldId, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        var result = recipeProfitRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns((RecipeProfitPoco)null);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        _ = await recipeProfitRepo.GetAsync(_worldId, _recipeId);

        _costCache.Received(2).Get((_worldId, _recipeId));
        _costCache
            .Received(1)
            .Add(
                (_worldId, _recipeId),
                Arg.Is<RecipeProfitPoco>(
                    recipeProfit => recipeProfit.WorldId == _worldId && recipeProfit.RecipeId == _recipeId
                )
            );
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfit = new RecipeProfitPoco { WorldId = _worldId, RecipeId = _recipeId };
        _costCache.Get((_worldId, _recipeId)).Returns(recipeProfit);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        _ = await recipeProfitRepo.GetAsync(_worldId, _recipeId);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);
        var allRecipeProfits = context.RecipeProfit.ToList();

        await recipeProfitRepo.FillCache();

        allRecipeProfits.ForEach(
            recipe =>
                _costCache
                    .Received(1)
                    .Add((recipe.WorldId, recipe.RecipeId), Arg.Any<RecipeProfitPoco>())
        );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.RecipeProfit.RemoveRange(context.RecipeProfit);
        context.SaveChanges();
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        await recipeProfitRepo.FillCache();

        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    {
        var poco = new RecipeProfitPoco { WorldId = _worldId, RecipeId = _recipeId };
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns(poco);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        await recipeProfitRepo.Add(poco);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        var poco = new RecipeProfitPoco { WorldId = _worldId, RecipeId = _recipeId };
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns((RecipeProfitPoco)null);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);

        await recipeProfitRepo.Add(poco);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.Received(1).Add((_worldId, _recipeId), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _costCache);
        var worldId = 77;
        var recipeId = 999;
        var recipeProfitVsListings = 333;
        var recipeProfitVsSold = 791;
        var poco = new RecipeProfitPoco
        {
            WorldId = worldId,
            RecipeId = recipeId,
            RecipeProfitVsListings = recipeProfitVsListings,
            RecipeProfitVsSold = recipeProfitVsSold,
            Updated = DateTimeOffset.Now
        };
        await recipeProfitRepo.Add(poco);

        var result = await recipeProfitRepo.GetAsync(worldId, recipeId);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId, Is.EqualTo(worldId));
            Assert.That(result.RecipeId, Is.EqualTo(recipeId));
            Assert.That(result.RecipeProfitVsSold, Is.EqualTo(recipeProfitVsSold));
            Assert.That(result.RecipeProfitVsListings, Is.EqualTo(recipeProfitVsListings));
            Assert.That(result.Updated.ToUnixTimeMilliseconds(), Is.GreaterThan(1695906560209));
        });
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _costCache = Substitute.For<IRecipeProfitCache>();
    }
}