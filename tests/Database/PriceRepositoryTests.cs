using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class PriceRepositoryTests : InMemoryTestDb
{
    private IPriceCache _cache;

    private static readonly int _worldID = 33;
    private static readonly int _itemID = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIDExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetAll(22);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.ItemID == 11));
            Assert.That(result.Any(p => p.ItemID == 12));
            Assert.That(!result.Any(p => p.WorldID != 22));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIDDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(22, id);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldID == 22);
            Assert.That(result.ItemID == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public void GivenAGet_WhenTheIDIsValidButNotTheWorldID_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIDIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new int[] { 11, 12 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.ItemID == 11));
            Assert.That(result.Any(p => p.ItemID == 12));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValidButNotWorldID_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(_worldID, new int[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new int[] { 11, 99 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.ItemID == 11));
            Assert.That(!result.Any(p => p.WorldID != 22));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new int[] { _worldID, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndNotCached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        _ = priceRepo.Get(_worldID, _itemID);

        _cache.Received(1).Get((_worldID, _itemID));
        _cache
            .Received(1)
            .Add(
                (_worldID, _itemID),
                Arg.Is<PricePoco>(price => price.WorldID == _worldID && price.ItemID == _itemID)
            );
    }

    [Test]
    public void GivenAGet_WhenTheIDIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var poco = new PricePoco { WorldID = _worldID, ItemID = _itemID };
        _cache.Get((_worldID, _itemID)).Returns((PricePoco)null, poco);
        var priceRepo = new PriceRepository(context, _cache);

        _ = priceRepo.Get(_worldID, _itemID);
        _ = priceRepo.Get(_worldID, _itemID);

        _cache.Received(2).Get((_worldID, _itemID));
        _cache
            .Received(1)
            .Add(
                (_worldID, _itemID),
                Arg.Is<PricePoco>(price => price.WorldID == _worldID && price.ItemID == _itemID)
            );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);
        var allPrices = context.Price.ToList();

        await priceRepo.FillCache();

        allPrices.ForEach(price => _cache.Received(1).Add((price.WorldID, price.ItemID), price));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.Price.RemoveRange(context.Price);
        context.SaveChanges();
        var priceRepo = new PriceRepository(context, _cache);

        await priceRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<(int, int)>(), Arg.Any<PricePoco>());
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _cache = Substitute.For<IPriceCache>();
    }
}
