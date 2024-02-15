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

public class PriceSaverTests : InMemoryTestDb
{
    private const int defaultWorldId = 34;

    private PriceSaver _saver;
    private ILogger<DataSaver<PricePoco>> _logger;
    private TestGilGoblinDbContext _context;

    [SetUp]
    public override void SetUp()
    {
        _logger = Substitute.For<ILogger<DataSaver<PricePoco>>>();
        base.SetUp();

        _context = new TestGilGoblinDbContext(_options, _configuration);
        _saver = new PriceSaver(_context, _logger);
    }

    [Test]
    public async Task GivenASaveAsync_WhenNoUpdatesAreSupplied_ThenWeReturnFalse()
    {
        var success = await _saver.SaveAsync(new List<PricePoco>());

        Assert.That(success, Is.False);
        _logger.DidNotReceiveWithAnyArgs().LogInformation(default);
        _logger.DidNotReceiveWithAnyArgs().LogError(default);
    }

    [Test]
    public async Task GivenASaveAsync_WhenAnUpdateIsValid_ThenWeLogSuccessAndReturnTrue()
    {
        var existing = _context.Price.First();
        var updates = new List<PricePoco> { existing };

        var success = await _saver.SaveAsync(updates);

        Assert.That(success);
        _logger.Received().LogInformation($"Saved {updates.Count} new entries for type {nameof(PricePoco)}");
    }

    [Test]
    public async Task GivenASaveAsync_WhenAnUpdateContainsEntities_ThenWeCheckNewVsExisting()
    {
        const int newEntityCount = 2;
        var initialPriceCount = _context.Price.Count();
        var existing = _context.Price.First();
        existing.AverageSold = 9887;
        var updates = GetNewPocos(newEntityCount);
        updates.Add(existing);

        var result = await _saver.SaveAsync(updates);

        var updatedEntity = await _context.Price.FindAsync(existing.ItemId, existing.WorldId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.AverageSold, Is.EqualTo(9887));
            Assert.That(_context.Price.Count(), Is.EqualTo(initialPriceCount + newEntityCount));
        });
    }

    [Test]
    public async Task GivenASaveAsync_WhenAnUpdateIsInvalid_ThenNothingISSaveAndAnErrorIsLogged()
    {
        const string infoMessage =
            "Failed to update due to invalid data: No valid entities remained after validity check";
        var existing = _context.Price.First();
        existing.ItemId = -1;
        var updates = new List<PricePoco> { existing };

        var success = await _saver.SaveAsync(updates);

        Assert.That(success, Is.False);
        _logger.Received().LogError(Arg.Any<Exception>(), infoMessage);
    }

    [Test]
    public async Task GivenASaveAsync_WhenUpdatesAreNewAndValid_ThenWeSaveTheData()
    {
        var updates = GetNewPocos(3);

        var success = await _saver.SaveAsync(updates);

        Assert.That(success);
        foreach (var update in updates)
        {
            var db = await _context.Price.FindAsync(update.ItemId, update.WorldId);

            Assert.Multiple(() =>
            {
                Assert.That(db, Is.Not.Null);
                Assert.That(db.AverageSold, Is.EqualTo(update.AverageSold));
                Assert.That(db.AverageListingPrice, Is.EqualTo(update.AverageListingPrice));
            });
        }
    }

    [TestCase(0, 0, 0)]
    [TestCase(0, 0, 1)]
    [TestCase(0, 1, 0)]
    [TestCase(0, 1, 1)]
    [TestCase(1, 1, 0)]
    public void GivenAnyFieldIsInvalid_WhenAnyFieldIsInvalid_ThenWeFailTheCheck(
        int worldId, int itemId, int lastUploadTime)
    {
        var updatesList = new List<PricePoco>
        {
            new()
            {
                WorldId = worldId,
                ItemId = itemId,
                LastUploadTime =
                    lastUploadTime == 0
                        ? 0
                        : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
        };

        Assert.That(_saver.SaveAsync(updatesList), Is.False);
    }

    private static List<PricePoco> GetNewPocos(int qty = 1)
    {
        var updates = new List<PricePoco>();
        for (var i = 0; i < qty; i++)
        {
            var pricePoco = new PricePoco
            {
                WorldId = defaultWorldId,
                ItemId = 100 + i * 111,
                AverageSold = 111f + i * 333,
                AverageListingPrice = 222f + i * 444,
                LastUploadTime = DateTimeOffset.UtcNow.AddDays(-3).ToUnixTimeMilliseconds()
            };
            updates.Add(pricePoco);
        }

        return updates;
    }
}