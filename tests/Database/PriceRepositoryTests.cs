using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class PriceRepositoryTests : InMemoryTestDb
{
    private IPriceCache _cache;

    private static readonly int _worldId = 33;
    private static readonly int _itemId = 88;

    [Test]
    public void GivenAGetAll_WhenTheWorldIdExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetAll(22);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.ItemId == 11));
            Assert.That(result.Any(p => p.ItemId == 12));
            Assert.That(!result.Any(p => p.WorldId != 22));
        });
    }

    [Test]
    public void GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(22, id);

        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId == 22);
            Assert.That(result.ItemId == id);
        });
    }

    [TestCase(11)]
    [TestCase(12)]
    public void GivenAGet_WhenTheIdIsValidButNotTheWorldId_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIdIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.Get(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new [] { 11, 12 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.ItemId == 11));
            Assert.That(result.Any(p => p.ItemId == 12));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValidButNotWorldId_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(_worldId, new [] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new [] { 11, 99 });

        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Any(p => p.ItemId == 11));
            Assert.That(!result.Any(p => p.WorldId != 22));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, new [] { _worldId, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        var result = priceRepo.GetMultiple(22, Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);

        _ = priceRepo.Get(_worldId, _itemId);

        _cache.Received(1).Get((_worldId, _itemId));
        _cache
            .Received(1)
            .Add(
                (_worldId, _itemId),
                Arg.Is<PricePoco>(price => price.WorldId == _worldId && price.ItemId == _itemId)
            );
    }

    [Test]
    public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var poco = new PricePoco { WorldId = _worldId, ItemId = _itemId };
        _cache.Get((_worldId, _itemId)).Returns(null, poco);
        var priceRepo = new PriceRepository(context, _cache);

        _ = priceRepo.Get(_worldId, _itemId);
        _ = priceRepo.Get(_worldId, _itemId);

        _cache.Received(2).Get((_worldId, _itemId));
        _cache
            .Received(1)
            .Add(
                (_worldId, _itemId),
                Arg.Is<PricePoco>(price => price.WorldId == _worldId && price.ItemId == _itemId)
            );
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context, _cache);
        var allPrices = context.Price.ToList();

        await priceRepo.FillCache();

        allPrices.ForEach(price => _cache.Received(1).Add((price.WorldId, price.ItemId), price));
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
