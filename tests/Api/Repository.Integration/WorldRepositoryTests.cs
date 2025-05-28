using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.Database.Integration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class WorldRepositoryTests : GilGoblinDatabaseFixture
{
    private IWorldCache _cache;
    private WorldRepository _worldRepo;

    [SetUp]
    public async Task SetUp()
    {
        await ResetAndRecreateDatabaseAsync();
        _cache = Substitute.For<IWorldCache>();

        _worldRepo = new WorldRepository(_serviceProvider, _cache);
    }

    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = GetDbContext();

        var result = _worldRepo.GetAll().ToList();

        var allWorlds = context.World.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(allWorlds.Count));
            allWorlds.ForEach(world => Assert.That(result.Any(p => p.Name == world.Name)));
            allWorlds.ForEach(world => Assert.That(result.Any(p => p.GetId() == world.GetId())));
        });
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        var result = _worldRepo.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(result?.Name, Has.Length.GreaterThan(0));
            Assert.That(result != null && result.GetId() == id);
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIdIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        var result = _worldRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        var result = _worldRepo.GetMultiple(ValidWorldIds).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidWorldIds.Count));
            Assert.That(result.All(w => ValidWorldIds.Contains(w.GetId())));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        var mixedIds = new[] { 99 }.Concat(ValidWorldIds).ToList();

        var result = _worldRepo.GetMultiple(mixedIds).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidWorldIds.Count));
            Assert.That(result.All(w => ValidWorldIds.Contains(w.GetId())));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenNoEntriesAreReturned()
    {
        var result = _worldRepo.GetMultiple(new[] { 654645646, 9953121 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenNoEntriesAreReturned()
    {
        var result = _worldRepo.GetMultiple(System.Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry(int worldId)
    {
        _ = _worldRepo.Get(worldId);

        _cache.Received(1).Get(worldId);
        _cache.Received(1).Add(worldId, Arg.Is<WorldPoco>(world => world.GetId() == worldId));
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntryTheSecondTime(int worldId)
    {
        _cache.Get(worldId).Returns(null, new WorldPoco { Id = worldId });

        _ = _worldRepo.Get(worldId);
        _ = _worldRepo.Get(worldId);

        _cache.Received(2).Get(worldId);
        _cache.Received(1).Add(worldId, Arg.Is<WorldPoco>(world => world.GetId() == worldId));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var dbContext = GetDbContext();
        var allWorlds = dbContext.World.ToList();

        await _worldRepo.FillCache();

        Assert.That(allWorlds, Has.Count.EqualTo(ValidWorldIds.Count));
        allWorlds.ForEach(world => _cache.Received(1).Add(world.GetId(), world));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = GetDbContext();
        context.World.RemoveRange(context.World);
        await context.SaveChangesAsync();

        await _worldRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<int>(), Arg.Any<WorldPoco>());
    }
}