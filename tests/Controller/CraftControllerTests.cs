using GilGoblin.Controller;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class CraftControllerTests
{
    private CraftController? _controller;
    private readonly ICraftRepository<CraftSummaryPoco> _repo = Substitute.For<
        ICraftRepository<CraftSummaryPoco>
    >();

    private static readonly int _world = 34;
    private static readonly int _craftId = 108;

    [SetUp]
    public void SetUp()
    {
        _controller = new CraftController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<CraftController>()
        );
        Assert.That(_controller, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _repo.ClearReceivedCalls();
    }

    [Test]
    public void WhenReceivingARequestGetBestCrafts_ThenTheRepositoryIsCalled()
    {
        _repo.GetBestCrafts(Arg.Any<int>()).Returns(new List<CraftSummaryPoco>());
        _ = _controller!.GetBestCrafts(_world);

        _repo.GetBestCrafts(_world).Received(1);
    }

    [Test]
    public void WhenReceivingARequestGetCraft_ThenTheRepositoryIsCalled()
    {
        _ = _controller!.GetCraft(_world, _craftId);

        _repo.GetCraft(_world, _craftId).Received(1);
    }

    // [Test]
    // public void WhenReceivingARequestGetBestCrafts_ThenTheAnEnumerableIsReturned()
    // {
    //     _repo.GetBestCrafts(Arg.Any<int>()).Returns(Array.Empty<CraftSummaryPoco>());

    //     var result = _controller!.GetBestCrafts(_world);

    //     Assert.That(result, Is.Not.Null);
    //     // Assert.That(result, Is.TypeOf<IEnumerable<CraftSummaryPoco>>());
    // }

    // [Test]
    // public void WhenReceivingARequestGetCraft_ThenAPocoIsReturned()
    // {
    //     var result = _controller!.GetBestCrafts(_defaultWorld);

    //     Assert.That(result, Is.Not.Null);
    //     Assert.That(result, Is.TypeOf<CraftSummaryPoco>());
    // }
}
