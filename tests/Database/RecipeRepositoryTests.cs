using GilGoblin.Database;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeRepositoryTests : InMemoryTestDb
{
    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context);

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
        var recipeRepo = new RecipeRepository(context);

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
        var recipeRepo = new RecipeRepository(context);

        var result = recipeRepo.GetRecipesForItem(targetItemID);

        Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context);

        var result = recipeRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context);

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
        var recipeRepo = new RecipeRepository(context);

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
        var recipeRepo = new RecipeRepository(context);

        var result = recipeRepo.GetMultiple(new int[] { 333, 999 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var recipeRepo = new RecipeRepository(context);

        var result = recipeRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

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
