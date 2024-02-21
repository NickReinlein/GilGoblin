using System.Linq;
using GilGoblin.Api.Repository;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class WorldRepositoryTests
{
    private WorldRepository _worldRepo;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _worldRepo = new WorldRepository();
    }

    [Test]
    public void GivenWeGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        var result = _worldRepo.GetAllWorlds();

        var list = result.Distinct().ToList();
        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.GreaterThanOrEqualTo(2));
        });
    }
}