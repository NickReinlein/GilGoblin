using System;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class ItemRepositoryTests : GilGoblinDatabaseFixture
{
    private IItemCache _cache;
    private ILogger<ItemRepository> _logger;
    private ItemRepository _itemRepo;

    [SetUp]
    public override async Task SetUp()
    {
        _cache = Substitute.For<IItemCache>();
        _logger = Substitute.For<ILogger<ItemRepository>>();

        await base.SetUp();
        _itemRepo = new ItemRepository(_serviceProvider, _cache, _logger);
    }

    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = GetDbContext();

        var result = _itemRepo.GetAll().ToList();

        var allItems = context.Item.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(allItems.Count));
            allItems.ForEach(item => Assert.That(result.Any(p => p.Name == item.Name)));
            allItems.ForEach(item => Assert.That(result.Any(p => p.Id == item.Id)));
            Assert.That(allItems, Has.Count.EqualTo(ValidItemsIds.Count));
        });
    }

    [TestCaseSource(nameof(ValidItemsIds))]
    public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        var ctx = GetDbContext();
        var item = ctx.Item.First(i => i.Id == id);

        var result = _itemRepo.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(item.Name));
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Level, Is.EqualTo(item.Level));
            Assert.That(result.StackSize, Is.EqualTo(item.StackSize));
            Assert.That(result.PriceLow, Is.EqualTo(item.PriceLow));
            Assert.That(result.PriceMid, Is.EqualTo(item.PriceMid));
            Assert.That(result.Description, Is.EqualTo(item.Description));
            Assert.That(result.CanHq, Is.EqualTo(item.CanHq));
            Assert.That(result.IconId, Is.EqualTo(item.IconId));
        });
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(9238192)]
    public void GivenAGet_WhenIdIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        var result = _itemRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGet_WhenTheDbThrowsAnException_ThenTheExceptionIsLoggedAndNullReturned()
    {
        const int itemId = 9238192;
        const string errorMessage = "Time dilation is too elevated";
        var serviceProviderWithErrors = Substitute.For<IServiceProvider>();
        serviceProviderWithErrors.GetService<GilGoblinDbContext>().ThrowsForAnyArgs(new Exception(errorMessage));
        var itemRepo = new ItemRepository(serviceProviderWithErrors, _cache, _logger);

        var result = itemRepo.Get(itemId);

        Assert.That(result, Is.Null);
        _logger.Received(1).Log(
            LogLevel.Warning,
            0,
            Arg.Is<object>(v => v.ToString()!.Contains($"Failed to get item {itemId}: {errorMessage}")),
            null,
            Arg.Any<Func<object, Exception, string>>()!);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        var result = _itemRepo.GetMultiple(ValidItemsIds).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidItemsIds.Count));
            Assert.That(ValidItemsIds.All(v => result.Any(p => p.Id == v)));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        var result = _itemRepo.GetMultiple(ValidItemsIds.Concat([99]));

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidItemsIds.Count));
            Assert.That(ValidItemsIds.All(v => result.Any(p => p.Id == v)));
            Assert.That(result.All(r => r.Id != 99));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenNoEntriesAreReturned()
    {
        var result = _itemRepo.GetMultiple([654645646, 9953121]);

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenNoEntriesAreReturned()
    {
        var result = _itemRepo.GetMultiple([]);

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(ValidItemsIds))]
    public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry(int id)
    {
        _ = _itemRepo.Get(id);

        _cache.Received(1).Get(id);
        _cache.Received(1).Add(id, Arg.Is<ItemPoco>(item => item.Id == id));
    }

    [TestCaseSource(nameof(ValidItemsIds))]
    public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry(int id)
    {
        _cache.Get(id).Returns(null, new ItemPoco { Id = id });

        _ = _itemRepo.Get(id);
        _ = _itemRepo.Get(id);

        _cache.Received(2).Get(id);
        _cache.Received(1).Add(id, Arg.Is<ItemPoco>(item => item.Id == id));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var context = GetDbContext();
        var allItems = context.Item.ToList();

        await _itemRepo.FillCache();

        allItems.ForEach(item => _cache.Received(1).Add(item.Id, item));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = GetDbContext();
        context.Item.RemoveRange(context.Item);
        await context.SaveChangesAsync();

        await _itemRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<int>(), Arg.Any<ItemPoco>());
    }
}