using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeCostRepositoryTests : InMemoryTestDb
{
    private IRecipeCostCache _costCache;

    private static readonly int _worldID = 33;
    private static readonly int _recipeID = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIDExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(22);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.RecipeID == 11));
            Assert.That(result.Any(p => p.RecipeID == 12));
            Assert.That(!result.Any(p => p.WorldID != 22));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIDDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(22, id);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldID == 22);
            Assert.That(result.RecipeID == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIDIsValidButNotTheWorldID_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIDIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = await recipeCostRepo.GetAsync(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { 11, 12 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.RecipeID == 11));
            Assert.That(result.Any(p => p.RecipeID == 12));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValidButNotWorldID_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(_worldID, new int[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { 11, 99 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.RecipeID == 11));
            Assert.That(!result.Any(p => p.WorldID != 22));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { _worldID, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        var result = recipeCostRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get(Arg.Any<(int, int)>()).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        _ = await recipeCostRepo.GetAsync(_worldID, _recipeID);

        _costCache.Received(1).Get((_worldID, _recipeID));
        _costCache
            .Received(1)
            .Add(
                (_worldID, _recipeID),
                Arg.Is<RecipeCostPoco>(
                    recipeCost => recipeCost.WorldID == _worldID && recipeCost.RecipeID == _recipeID
                )
            );
    }

    [Test]
    public async Task GivenAGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCost = new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID };
        _costCache = Substitute.For<IRecipeCostCache>();
        _costCache.Get((_worldID, _recipeID)).Returns((RecipeCostPoco)null, recipeCost);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);
        await recipeCostRepo.FillCache();

        _ = await recipeCostRepo.GetAsync(_worldID, _recipeID);

        _costCache.Received(2).Get((_worldID, _recipeID));
        _costCache
            .Received(1)
            .Add(
                (_worldID, _recipeID),
                Arg.Is<RecipeCostPoco>(
                    recipeCost => recipeCost.WorldID == _worldID && recipeCost.RecipeID == _recipeID
                )
            );
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
                    .Add((recipe.WorldID, recipe.RecipeID), Arg.Any<RecipeCostPoco>())
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
        var poco = new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID };
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldID, _recipeID)).Returns(poco);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((_worldID, _recipeID));
        _costCache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        var poco = new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID };
        using var context = new GilGoblinDbContext(_options, _configuration);
        _costCache.Get((_worldID, _recipeID)).Returns((RecipeCostPoco)null);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);

        await recipeCostRepo.Add(poco);

        _costCache.Received(1).Get((_worldID, _recipeID));
        _costCache.Received(1).Add((_worldID, _recipeID), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _costCache);
        var poco = new RecipeCostPoco { WorldID = 77, RecipeID = 999 };
        await recipeCostRepo.Add(poco);

        var result = await recipeCostRepo.GetAsync(77, 999);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldID, Is.EqualTo(77));
            Assert.That(result.RecipeID, Is.EqualTo(999));
        });
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _costCache = Substitute.For<IRecipeCostCache>();
    }
}
