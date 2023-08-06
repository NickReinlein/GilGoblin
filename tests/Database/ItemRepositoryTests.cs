using GilGoblin.Database;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class ItemRepositoryTests : RepositoryTests
{
    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.GetAll();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, result.Count());
            Assert.That(result.Any(p => p.Name == "Item 1"));
            Assert.That(result.Any(p => p.Name == "Item 2"));
        });
    }

    [TestCase(1)]
    [TestCase(2)]
    public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name == $"Item {id}");
            Assert.That(result.ID == id);
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.GetMultiple(new int[] { 1, 2 });

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, result.Count());
            Assert.That(result.Any(p => p.ID == 1));
            Assert.That(result.Any(p => p.ID == 2));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.GetMultiple(new int[] { 1, 99 });

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, result.Count());
            Assert.That(result.Any(p => p.ID == 1));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.GetMultiple(new int[] { 33, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        var context = new GilGoblinDbContext(_options);
        context.ItemInfo.AddRange(
            new ItemInfoPoco { ID = 1, Name = "Item 1" },
            new ItemInfoPoco { ID = 2, Name = "Item 2" }
        );
        context.SaveChanges();
    }
}
