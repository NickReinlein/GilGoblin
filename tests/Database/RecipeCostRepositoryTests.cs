using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeCostRepositoryTests : InMemoryTestDb
{
    private IRecipeCostCache _cache;

    private static readonly int _worldID = 33;
    private static readonly int _recipeID = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIDExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

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
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = recipeCostRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public async Task GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

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
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = await recipeCostRepo.GetAsync(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public async Task GivenAGet_WhenIDIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = await recipeCostRepo.GetAsync(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

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
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = recipeCostRepo.GetMultiple(_worldID, new int[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

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
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = recipeCostRepo.GetMultiple(22, new int[] { _worldID, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        var result = recipeCostRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public async Task GivenAGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        _ = await recipeCostRepo.GetAsync(_worldID, _recipeID);

        _cache.Received(1).Get((_worldID, _recipeID));
        _cache
            .Received(1)
            .Add(
                (_worldID, _recipeID),
                Arg.Is<RecipeCostPoco>(
                    RecipeCost => RecipeCost.WorldID == _worldID && RecipeCost.RecipeID == _recipeID
                )
            );
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        _ = recipeCostRepo.GetAsync(_worldID, _recipeID);
        _ = recipeCostRepo.GetAsync(_worldID, _recipeID);

        _cache.Received(2).Get((_worldID, _recipeID));
        _cache
            .Received(1)
            .Add(
                (_worldID, _recipeID),
                Arg.Is<RecipeCostPoco>(
                    RecipeCost => RecipeCost.WorldID == _worldID && RecipeCost.RecipeID == _recipeID
                )
            );
    }

    [Test]
    public void GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);
        var allRecipeCosts = context.RecipeCost.ToList();

        recipeCostRepo.FillCache();

        allRecipeCosts.ForEach(
            RecipeCost =>
                _cache.Received(1).Add((RecipeCost.WorldID, RecipeCost.RecipeID), RecipeCost)
        );
    }

    [Test]
    public void GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.SaveChanges();
        var recipeCostRepo = new RecipeCostRepository(context, _cache);

        recipeCostRepo.FillCache();

        _cache.DidNotReceive().Add((Arg.Any<int>(), Arg.Any<int>()), Arg.Any<RecipeCostPoco>());
        FillTable();
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryExists_ThenWeReturnItAndDoNotAddToCacheAgain()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);
        var poco = new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID };

        await recipeCostRepo.Add(poco);

        _cache.Received(1).Get((poco.WorldID, poco.RecipeID));
        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<RecipeCostPoco>());
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeCacheIt()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);
        var poco = new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID };

        await recipeCostRepo.Add(poco);

        _cache.Received(1).Get((_worldID, _recipeID));
        _cache.Received(1).Add((_worldID, _recipeID), poco);
    }

    [Test]
    public async Task GivenAnAdd_WhenEntryIsNew_ThenWeSaveItToTheDatabase()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeCostRepo = new RecipeCostRepository(context, _cache);
        var poco = new RecipeCostPoco { WorldID = 77, RecipeID = 999 };
        await recipeCostRepo.Add(poco);

        var result = await recipeCostRepo.GetAsync(77, 999);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldID == 77);
            Assert.That(result.RecipeID == 999);
        });
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        _cache = Substitute.For<RecipeCostCache>();

        FillTable();
    }

    private void FillTable()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.RecipeCost.AddRange(
            new RecipeCostPoco { WorldID = 22, RecipeID = 11 },
            new RecipeCostPoco { WorldID = 22, RecipeID = 12 },
            new RecipeCostPoco { WorldID = _worldID, RecipeID = _recipeID },
            new RecipeCostPoco { WorldID = 44, RecipeID = 99 }
        );
        context.SaveChanges();
    }
}
