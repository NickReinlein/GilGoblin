using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeCostRepositoryTests : InMemoryTestDb
{
    private IRecipeCostCache _costCache;

    private static readonly int _worldId = 33;
    private static readonly int _recipeId = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(22);

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
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(22, id);

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
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { 11, 12 });

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
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(_worldId, new int[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { 11, 99 });

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
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { _worldId, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        _ = await recipeCostRepo.GetAsync(_worldId, _recipeId);

        _costCache.Received(2).Get((_worldId, _recipeId));
        _costCache
            .Received(1)
            .Add(
                (_worldId, _recipeId),
                Arg.Is<RecipeCostPoco>(
                    recipeCost => recipeCost.WorldId == _worldId && recipeCost.RecipeId == _recipeId
                )
            );
    }

    [Test]
    public async Task GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCost = new RecipeCostPoco { WorldId = _worldId, RecipeId = _recipeId };
        _costCache.Get((_worldId, _recipeId)).Returns(recipeCost);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        _ = await recipeCostRepo.GetAsync(_worldId, _recipeId);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
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
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.SaveChanges();
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.FillCache();

        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    {
        var poco = new RecipeCostPoco { WorldId = _worldId, RecipeId = _recipeId };
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns(poco);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        var poco = new RecipeCostPoco { WorldId = _worldId, RecipeId = _recipeId };
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldId, _recipeId)).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((_worldId, _recipeId));
        _costCache.Received(1).Add((_worldId, _recipeId), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
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
    }
}
