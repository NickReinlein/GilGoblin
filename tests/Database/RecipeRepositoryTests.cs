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
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
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
    public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result.ID == id);
    }

    [TestCase(111, 2)]
    [TestCase(222, 1)]
    [TestCase(333, 1)]
    public void GivenAGetRecipesForItem_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntries(
        int targetItemID,
        int expectedResultsCount
    )
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetRecipesForItem(targetItemID);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(99999)]
    public void GivenAGet_WhenIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
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
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
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
    public void GivenAGetMultiple_WhenIDsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { 333, 999 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        var result = recipeRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);
        const int recipeID = 44;

        _ = recipeRepo.Get(recipeID);

        _recipeCache.Received(1).Get(recipeID);
        _recipeCache.Received(1).Add(recipeID, Arg.Is<RecipePoco>(recipe => recipe.ID == recipeID));
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);
        const int recipeID = 44;

        _ = recipeRepo.Get(recipeID);
        _ = recipeRepo.Get(recipeID);

        _recipeCache.Received(2).Get(recipeID);
        _recipeCache.Received(1).Add(recipeID, Arg.Is<RecipePoco>(recipe => recipe.ID == recipeID));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);
        var allRecipes = context.Recipe.ToList();

        await recipeRepo.FillCache();

        allRecipes.ForEach(recipe => _recipeCache.Received(1).Add(recipe.ID, recipe));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.Recipe.RemoveRange(context.Recipe);
        context.SaveChanges();
        var recipeRepo = new RecipeRepository(context, _recipeCache, _itemRecipeCache);

        await recipeRepo.FillCache();

        _recipeCache.DidNotReceive().Add(Arg.Any<int>(), Arg.Any<RecipePoco>());
        FillTable();
    }

    [SetUp]
    public void Setup()
    {
        _recipeCache = Substitute.For<RecipeCache>();
        _itemRecipeCache = Substitute.For<ItemRecipeCache>();
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
