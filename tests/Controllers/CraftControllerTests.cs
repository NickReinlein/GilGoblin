using GilGoblin.Controllers;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controllers;

public class CraftControllerTests
{
    private CraftController? _controller;
    private ICraftRepository<CraftSummaryPoco> _repo;

    private static readonly int _world = 34;
    private static readonly int _craftId = 108;

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<ICraftRepository<CraftSummaryPoco>>();

        _controller = new CraftController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<CraftController>()
        );
    }

    [Test]
    public void WhenWeSetup_ControllerIsSucessfullyCreated()
    {
        Assert.That(_controller, Is.Not.Null);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCrafts_ThenTheRepositoryIsCalled()
    {
        _repo.GetBestCrafts(_world).Returns(new List<CraftSummaryPoco>());

        _ = await _controller!.GetBestCrafts(_world);

        await _repo.Received(1).GetBestCrafts(_world);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCraft_ThenTheRepositoryIsCalled()
    {
        _ = await _controller!.GetBestCraft(_world, _craftId);

        await _repo.Received(1).GetBestCraft(_world, _craftId);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCrafts_ThenAnEnumerableIsReturned()
    {
        _repo.GetBestCrafts(_world).Returns(new List<CraftSummaryPoco>());

        var result = await _controller!.GetBestCrafts(_world);

        Assert.That(result is not null);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCraft_ThenASummaryIsReturned()
    {
        _repo.GetBestCraft(_world, _craftId).Returns(new CraftSummaryPoco());

        var result = await _controller!.GetBestCraft(_world, _craftId);

        Assert.That(result, Is.TypeOf<CraftSummaryPoco>());
    }
}
