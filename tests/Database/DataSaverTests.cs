using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class DataSaverTests : InMemoryTestDb
{
    private const int defaultItemId = 1604;

    private DataSaver<ItemPoco> _saver;
    private ILogger<DataSaver<ItemPoco>> _logger;
    private TestGilGoblinDbContext _context;

    [SetUp]
    public override void SetUp()
    {
        _logger = Substitute.For<ILogger<DataSaver<ItemPoco>>>();
        base.SetUp();

        _context = new TestGilGoblinDbContext(_options, _configuration);
        _saver = new DataSaver<ItemPoco>(_context, _logger);
    }

    [Test]
    public async Task GivenASaveAsync_WhenNoUpdatesAreSupplied_ThenWeReturnFalse()
    {
        var success = await _saver.SaveAsync(new List<ItemPoco>());

        Assert.That(success, Is.False);
        _logger.DidNotReceiveWithAnyArgs().LogInformation(default);
        _logger.DidNotReceiveWithAnyArgs().LogError(default);
    }

    [Test]
    public async Task GivenASaveAsync_WhenAnUpdateIsValid_ThenWeReturnTrue()
    {
        var updates = GetPocos();

        var success = await _saver.SaveAsync(updates);

        Assert.That(success);
    }

    [Test]
    public async Task GivenASaveAsync_WhenAnUpdateContainsEntities_ThenWeCheckNewVsExisting()
    {
        const int newEntityCount = 2;
        var initialCount = _context.Item.Count();
        var existing = _context.Item.First();
        existing.PriceMid = 9887;
        var updates = GetPocos(newEntityCount);
        updates.Add(existing);

        var success = await _saver.SaveAsync(updates);
        Assert.That(success);

        var updatedEntity = await _context.Item.FindAsync(existing.GetId());
        Assert.Multiple(() =>
        {
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.PriceMid, Is.EqualTo(9887));
            Assert.That(_context.Item.Count(), Is.EqualTo(initialCount + newEntityCount));
        });
    }

    // [Test]
    // public async Task GivenASaveAsync_WhenAnUpdateIsInvalid_ThenWeLogAnErrorAndReturnFalse()
    // {
    //     const string errorMessage = "Failed to update due to error: Cannot save price due to error in key field";
    //     var updates = GetPocos();
    //     updates.First().ItemId = -1;
    //
    //     var success = await _saver.SaveAsync(updates);
    //
    //     Assert.That(success, Is.False);
    //     _logger.Received().LogError(errorMessage);
    // }
    //
    // [Test]
    // public async Task GivenASaveAsync_WhenUpdatesAreNewAndValid_ThenWeSaveTheData()
    // {
    //     var updates = GetPocos(3);
    //
    //     var success = await _saver.SaveAsync(updates);
    //
    //     Assert.That(success);
    //     foreach (var update in updates)
    //     {
    //         var db = await _context.Price.FindAsync(update.ItemId, update.WorldId);
    //
    //         Assert.Multiple(() =>
    //         {
    //             Assert.That(db, Is.Not.Null);
    //             Assert.That(db.AverageSold, Is.EqualTo(update.AverageSold));
    //             Assert.That(db.AverageListingPrice, Is.EqualTo(update.AverageListingPrice));
    //         });
    //     }
    // }

    private static List<ItemPoco> GetPocos(int qty = 1)
    {
        var updates = new List<ItemPoco>();
        for (var i = 0; i < qty; i++)
        {
            var item = new ItemPoco { Id = defaultItemId + i * 227, PriceMid = 33 + i * 333, PriceLow = 13 + i * 27 };
            updates.Add(item);
        }

        return updates;
    }
}