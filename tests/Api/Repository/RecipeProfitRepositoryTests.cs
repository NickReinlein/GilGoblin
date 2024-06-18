using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class RecipeProfitRepositoryTests : PriceDependentTests
{
    private IRecipeProfitCache _profitCache;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _profitCache = Substitute.For<IRecipeProfitCache>();
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetAll(WorldId);

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
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = await recipeProfitRepo.GetAsync(WorldId, id);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId == WorldId);
            Assert.That(result.RecipeId == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValidButNotTheWorldId_ThenNullIsReturned(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = await recipeProfitRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = await recipeProfitRepo.GetAsync(WorldId, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetMultiple(WorldId, new[] { RecipeId, RecipeId2 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(2));
            Assert.That(result.Any(p => p.RecipeId == RecipeId));
            Assert.That(result.Any(p => p.RecipeId == RecipeId2));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetMultiple(654564654, new[] { RecipeId, RecipeId2 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetMultiple(WorldId, new[] { RecipeId, 95459 });

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
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetMultiple(WorldId, new[] { 654645646, 9953121 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        var result = recipeProfitRepo.GetMultiple(WorldId, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _profitCache.Get((WorldId, RecipeId)).Returns((RecipeProfitPoco)null!);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        _ = await recipeProfitRepo.GetAsync(WorldId, RecipeId);

        _profitCache.Received().Get((WorldId, RecipeId));
        _profitCache
            .Received(1)
            .Add(
                (WorldId, RecipeId),
                Arg.Is<RecipeProfitPoco>(
                    recipeProfit => recipeProfit.WorldId == WorldId &&
                                    recipeProfit.RecipeId == RecipeId
                )
            );
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        var recipeProfit = GetRecipeProfitPoco();
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _profitCache.Get((WorldId, RecipeId)).Returns(recipeProfit);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        _ = await recipeProfitRepo.GetAsync(WorldId, RecipeId);

        _profitCache.Received().Get((WorldId, RecipeId));
        _profitCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);
        var allRecipeProfits = context.RecipeProfit.ToList();

        await recipeProfitRepo.FillCache();

        allRecipeProfits.ForEach(
            recipe =>
                _profitCache
                    .Received(1)
                    .Add((recipe.WorldId, recipe.RecipeId), Arg.Any<RecipeProfitPoco>())
        );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.RecipeProfit.RemoveRange(context.RecipeProfit);
        await context.SaveChangesAsync();
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        await recipeProfitRepo.FillCache();

        _profitCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    {
        var poco = GetRecipeProfitPoco();
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _profitCache.Get((WorldId, RecipeId)).Returns(poco);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        await recipeProfitRepo.AddAsync(poco);

        _profitCache.Received(1).Get((WorldId, RecipeId));
        _profitCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        var poco = GetRecipeProfitPoco();
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        _profitCache.Get((WorldId, RecipeId)).Returns((RecipeProfitPoco)null!);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);

        await recipeProfitRepo.AddAsync(poco);

        _profitCache.Received(1).Get((WorldId, RecipeId));
        _profitCache.Received(1).Add((WorldId, RecipeId), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeProfitRepo = new RecipeProfitRepository(context, _profitCache);
        var poco = new RecipeProfitPoco
        {
            WorldId = 77,
            RecipeId = 65454,
            RecipeProfitVsListings = 3315,
            RecipeProfitVsSold = 6454,
            Updated = DateTime.Now
        };

        await recipeProfitRepo.AddAsync(poco);

        var result = await recipeProfitRepo.GetAsync(poco.WorldId, poco.RecipeId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.WorldId, Is.EqualTo(poco.WorldId));
            Assert.That(result.RecipeId, Is.EqualTo(poco.RecipeId));
            Assert.That(result.RecipeProfitVsSold, Is.EqualTo(poco.RecipeProfitVsSold));
            Assert.That(result.RecipeProfitVsListings, Is.EqualTo(poco.RecipeProfitVsListings));
            Assert.That(result.Updated.ToUniversalTime(), Is.GreaterThan(DateTimeOffset.UtcNow.AddMinutes(-1)));
        });
    }

    private static RecipeProfitPoco GetRecipeProfitPoco()
    {
        return new RecipeProfitPoco
        {
            WorldId = WorldId,
            RecipeId = RecipeId,
            RecipeProfitVsListings = 100,
            RecipeProfitVsSold = 200,
            Updated = DateTime.Now
        };
    }
}