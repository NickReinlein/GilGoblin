using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeRepositoryTests : InMemoryTestDb
{
    private IRecipeCache _recipeCache;
    private IItemRecipeCache _itemRecipeCache;

    [Test]
    public void GivenWeGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result.Any(p => p.ID == 11));
            Assert.That(result.Any(p => p.ID == 22));
            Assert.That(result.Any(p => p.ID == 33));
            Assert.That(result.Any(p => p.ID == 44));
        });
    }

    [TestCase(11)]
    [TestCase(22)]
    [TestCase(33)]
    [TestCase(44)]
    public void GivenWeGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result.ID == id);
    }

    [TestCase(111, 2)]
    [TestCase(222, 1)]
    [TestCase(333, 1)]
    public void GivenWeGetRecipesForItem_WhenTheIDIsValidAndUncached_ThenTheRepositoryReturnsTheCorrectEntriesAndCachesThem(
        int targetItemID,
        int expectedResultsCount
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemID);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
        _itemRecipeCache.Received(1).Get(targetItemID);
        _itemRecipeCache.Received(1).Add(targetItemID, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(111, 2)]
    [TestCase(222, 1)]
    [TestCase(333, 1)]
    public void GivenGetRecipesForItem_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntryImmediately(
        int targetItemID,
        int expectedResultsCount
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var pocos = context.Recipe.Where(r => r.TargetItemID == targetItemID).ToList();
        _itemRecipeCache.Get(targetItemID).Returns(pocos);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemID);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
        _itemRecipeCache.Received(1).Get(targetItemID);
        _itemRecipeCache.DidNotReceive().Add(targetItemID, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void GivenWeGetRecipesForItem_WhenTheIDIsInvalid_ThenTheRepositoryReturnsAnEmptyResultImmediately(
        int targetItemID
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemID);

        Assert.That(!result.Any());
        _itemRecipeCache.DidNotReceive().Get(targetItemID);
        _itemRecipeCache.DidNotReceive().Add(targetItemID, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(99999)]
    public void GivenWeGet_WhenIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenWeGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 11, 33, 44 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(p => p.ID == 11));
            Assert.That(result.Any(p => p.ID == 33));
            Assert.That(result.Any(p => p.ID == 44));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 22, 857 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.ID == 22));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenIDsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 333, 999 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenWeGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenWeGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        var recipeID = 44;
        using var context = new GilGoblinDbContext(_options, _configuration);
        _recipeCache = Substitute.For<IRecipeCache>();
        _recipeCache.Get(recipeID).Returns((RecipePoco)null);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        _ = recipeRepo.Get(recipeID);

        _recipeCache.Received(1).Get(recipeID);
        _recipeCache.Received(1).Add(recipeID, Arg.Is<RecipePoco>(recipe => recipe.ID == recipeID));
    }

    [Test]
    public void GivenWeGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeID = 44;
        var poco = new RecipePoco { ID = recipeID };
        _recipeCache.Get(recipeID).Returns(poco);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(recipeID);

        Assert.That(result, Is.EqualTo(poco));
        _recipeCache.Received(1).Get(recipeID);
        _recipeCache.DidNotReceive().Add(recipeID, Arg.Any<RecipePoco>());
    }

    [Test]
    public async Task GivenWeFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);
        var allRecipes = context.Recipe.ToList();

        await recipeRepo.FillCache();

        allRecipes.ForEach(recipe => _recipeCache.Received(1).Add(recipe.ID, recipe));
    }

    [SetUp]
    public void Setup()
    {
        _recipeCache = Substitute.For<IRecipeCache>();
        _itemRecipeCache = Substitute.For<IItemRecipeCache>();
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();
        FillTable();
    }

    private void FillTable()
    {
        var context = new GilGoblinDbContext(_options, _configuration);
        context.Recipe.AddRange(
            new RecipePoco { ID = 11, TargetItemID = 111 },
            new RecipePoco { ID = 22, TargetItemID = 111 },
            new RecipePoco { ID = 33, TargetItemID = 222 },
            new RecipePoco { ID = 44, TargetItemID = 333 }
        );
        context.SaveChanges();
    }
}
