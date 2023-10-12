using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using NSubstitute;
using NSubstitute.Extensions;
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
            Assert.That(result.Any(p => p.Id == 11));
            Assert.That(result.Any(p => p.Id == 22));
            Assert.That(result.Any(p => p.Id == 33));
            Assert.That(result.Any(p => p.Id == 44));
        });
    }

    [TestCase(11)]
    [TestCase(22)]
    [TestCase(33)]
    [TestCase(44)]
    public void GivenWeGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result.Id == id);
    }

    [TestCase(111, 2)]
    [TestCase(222, 1)]
    [TestCase(333, 1)]
    public void GivenWeGetRecipesForItem_WhenTheIdIsValidAndUncached_ThenTheRepositoryReturnsTheCorrectEntriesAndCachesThem(
        int targetItemId,
        int expectedResultsCount
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemId);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
        _itemRecipeCache.Received(1).Get(targetItemId);
        _itemRecipeCache.Received(1).Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(111, 2)]
    [TestCase(222, 1)]
    [TestCase(333, 1)]
    public void GivenGetRecipesForItem_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntryImmediately(
        int targetItemId,
        int expectedResultsCount
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipes = context.Recipe.Where(r => r.TargetItemId == targetItemId).ToList();
        _itemRecipeCache.Configure().Get(Arg.Is(targetItemId)).Returns(recipes);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemId);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
        _itemRecipeCache.Received(1).Get(targetItemId);
        _itemRecipeCache.DidNotReceive().Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void GivenWeGetRecipesForItem_WhenTheIdIsInvalid_ThenTheRepositoryReturnsAnEmptyResultImmediately(
        int targetItemId
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemId);

        Assert.That(!result.Any());
        _itemRecipeCache.DidNotReceive().Get(targetItemId);
        _itemRecipeCache.DidNotReceive().Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(99999)]
    public void GivenWeGet_WhenIdIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenWeGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 11, 33, 44 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(p => p.Id == 11));
            Assert.That(result.Any(p => p.Id == 33));
            Assert.That(result.Any(p => p.Id == 44));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 22, 857 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.Id == 22));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenIdsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 333, 999 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenWeGetMultiple_WhenIdsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenWeGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        var recipeId = 44;
        using var context = new GilGoblinDbContext(_options, _configuration);
        _recipeCache.Get(recipeId).Returns((RecipePoco)null);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        _ = recipeRepo.Get(recipeId);

        _recipeCache.Received(1).Get(recipeId);
        _recipeCache.Received(1).Add(recipeId, Arg.Is<RecipePoco>(recipe => recipe.Id == recipeId));
    }

    [Test]
    public void GivenWeGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeId = 44;
        var poco = new RecipePoco { Id = recipeId };
        _recipeCache.Get(recipeId).Returns(poco);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(recipeId);

        Assert.That(result, Is.EqualTo(poco));
        _recipeCache.Received(1).Get(recipeId);
        _recipeCache.DidNotReceive().Add(recipeId, Arg.Any<RecipePoco>());
    }

    [Test]
    public async Task GivenWeFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);
        var allRecipes = context.Recipe.ToList();

        await recipeRepo.FillCache();

        allRecipes.ForEach(recipe => _recipeCache.Received(1).Add(recipe.Id, recipe));
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _recipeCache = Substitute.For<IRecipeCache>();
        _itemRecipeCache = Substitute.For<IItemRecipeCache>();
    }
}
