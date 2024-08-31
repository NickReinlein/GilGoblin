using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using GilGoblin.Tests.InMemoryTest;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class WorldRepositoryTests : InMemoryTestDb
{
    private IWorldCache _cache;

    [Test]
    public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.GetAll().ToList();

        var allWorlds = context.World.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(allWorlds.Count));
            allWorlds.ForEach(world => Assert.That(result.Any(p => p.Name == world.Name)));
            allWorlds.ForEach(world => Assert.That(result.Any(p => p.Id == world.Id)));
        });
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.Get(id);

        Assert.Multiple(() =>
        {
            Assert.That(result?.Name, Has.Length.GreaterThan(0));
            Assert.That(result != null && result.Id == id);
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(100)]
    public void GivenAGet_WhenIdIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.GetMultiple(ValidWorldIds).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidWorldIds.Count));
            Assert.That(result.All(w => ValidWorldIds.Contains(w.Id)));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);
        var mixedIds = new[] { 99 }.Concat(ValidWorldIds).ToList();

        var result = worldRepo.GetMultiple(mixedIds).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidWorldIds.Count));
            Assert.That(result.All(w => ValidWorldIds.Contains(w.Id)));
        });
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsAreInvalid_ThenNoEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.GetMultiple(new[] { 654645646, 9953121 });

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenAGetMultiple_WhenIdsEmpty_ThenNoEntriesAreReturned()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        var result = worldRepo.GetMultiple(System.Array.Empty<int>());

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry(int worldId)
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);

        _ = worldRepo.Get(worldId);

        _cache.Received(1).Get(worldId);
        _cache.Received(1).Add(worldId, Arg.Is<WorldPoco>(world => world.Id == worldId));
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public void GivenAGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntryTheSecondTime(int worldId)
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);
        _cache.Get(worldId).Returns(null, new WorldPoco { Id = worldId });

        _ = worldRepo.Get(worldId);
        _ = worldRepo.Get(worldId);

        _cache.Received(2).Get(worldId);
        _cache.Received(1).Add(worldId,
            Arg.Is<WorldPoco>(world => world.Id == worldId));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var worldRepo = new WorldRepository(context, _cache);
        var allWorlds = context.World.ToList();

        await worldRepo.FillCache();

        allWorlds.ForEach(world => _cache.Received(1).Add(world.Id, world));
    }

    [Test]
    public async Task GivenAFillCache_WhenEntriesDoNotExist_ThenWeDoNothing()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.World.RemoveRange(context.World);
        await context.SaveChangesAsync();
        var worldRepo = new WorldRepository(context, _cache);

        await worldRepo.FillCache();

        _cache.DidNotReceive().Add(Arg.Any<int>(), Arg.Any<WorldPoco>());
    }

    [SetUp]
    public override void SetUp()
    {
        _cache = Substitute.For<IWorldCache>();
        base.SetUp();
    }
}