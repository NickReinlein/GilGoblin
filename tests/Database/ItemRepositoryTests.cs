using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class ItemRepositoryTests : InMemoryTestDb
{
    private IItemInfoCache _cache;

    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.Name == "Item 1"));
            Assert.That(result.Any(p => p.Name == "Item 2"));
        });
    }

    [TestCase(1)]
    [TestCase(2)]
    public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

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
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.GetMultiple(new int[] { 1, 2 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.ID == 1));
            Assert.That(result.Any(p => p.ID == 2));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.GetMultiple(new int[] { 1, 99 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.ID == 1));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.GetMultiple(new int[] { 33, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        var result = itemRepo.GetMultiple(new int[] { });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);

        _ = itemRepo.Get(2);

        _cache.Received(1).Get(2);
        _cache.Received(1).Add(2, Arg.Is<ItemInfoPoco>(item => item.ID == 2));
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);
        _cache.Get(2).Returns(null, new ItemInfoPoco() { ID = 2 });
        _ = itemRepo.Get(2);

        var item = itemRepo.Get(2);

        _cache.Received(2).Get(2);
        _cache.Received(1).Add(2, Arg.Is<ItemInfoPoco>(item => item.ID == 2));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var itemRepo = new ItemRepository(context, _cache);
        var allItems = context.ItemInfo.ToList();

        await itemRepo.FillCache();

        allItems.ForEach(item => _cache.Received(1).Add(item.ID, item));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.ItemInfo.RemoveRange(context.ItemInfo);
        context.SaveChanges();
        var itemRepo = new ItemRepository(context, _cache);

        await itemRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<int>(), Arg.Any<ItemInfoPoco>());
        FillTable();
    }

    [SetUp]
    public void Setup()
    {
        _cache = Substitute.For<IItemInfoCache>();
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();
        FillTable();
    }

    private void FillTable()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.ItemInfo.AddRange(
            new ItemInfoPoco { ID = 1, Name = "Item 1" },
            new ItemInfoPoco { ID = 2, Name = "Item 2" }
        );
        context.SaveChanges();
    }
}
