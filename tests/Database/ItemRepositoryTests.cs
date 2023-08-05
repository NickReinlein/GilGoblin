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
    public void GivenAGet_WhenTheIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new GilGoblinDbContext(_options);
        var itemRepo = new ItemRepository(context);

        var result = itemRepo.Get(id);

        Assert.That(result, Is.Null);
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

// [Test]
// public void GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
// {
//     var multiple = Enumerable.Range(1, 10);
//     _itemRepository.GetMultiple(multiple);

//     _itemRepository.Received(1).GetMultiple(multiple);
// }

// [Test]
// public void GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
// {
//     _itemRepository.GetAll();

//     _itemRepository.Received(1).GetAll();
// }
// }
