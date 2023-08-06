using GilGoblin.Database;
using GilGoblin.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class PriceRepositoryTests : RepositoryTests
{
    [Test]
    public void GivenAGetAll_WhenTheWorldIDExists_ThenTheRepositoryReturnsAllEntriesForThatWorld()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

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
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.GetAll(999);

        Assert.That(!result.Any());
    }

    [TestCase(11)]
    [TestCase(12)]
    public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

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
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.Get(854, id);

        Assert.That(result, Is.Null);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIDIsInvalid_ThenNullIsReturned(int id)
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.Get(22, id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

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
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.GetMultiple(33, new int[] { 11, 12 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

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
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.GetMultiple(22, new int[] { 33, 99 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIDsEmpty_ThenAnEmptyResponseIsReturned()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        var priceRepo = new PriceRepository(context);

        var result = priceRepo.GetMultiple(22, new int[] { });

        Assert.That(!result.Any());
    }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        var context = new GilGoblinDbContext(_options, _configuration);
        context.Price.AddRange(
            new PricePoco { WorldID = 22, ItemID = 11 },
            new PricePoco { WorldID = 22, ItemID = 12 },
            new PricePoco { WorldID = 33, ItemID = 88 },
            new PricePoco { WorldID = 44, ItemID = 99 }
        );
        context.SaveChanges();
    }
}
