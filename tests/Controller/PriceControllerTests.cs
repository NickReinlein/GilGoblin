using GilGoblin.Controller;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class PriceControllerTests
{
    private PriceController _controller;
    private readonly IPriceRepository<PricePoco> _repo = Substitute.For<
        IPriceRepository<PricePoco>
    >();

    private static readonly int _itemId = 108;
    private static readonly int _worldId = 34;

    [SetUp]
    public void SetUp()
    {
        _controller = new PriceController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<PriceController>()
        );
        Assert.That(_controller, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _repo.ClearReceivedCalls();
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAnEnumerableResultIsReturned()
    {
        _repo.GetAll(_worldId).Returns(new List<PricePoco>());

        var result = _controller.GetAll(_worldId);

        Assert.That(result is IEnumerable<PricePoco>);
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenOneResultIsReturned()
    {
        _repo.Get(_worldId, _itemId).Returns(new PricePoco());

        var result = _controller.Get(_worldId, _itemId);

        Assert.That(result is PricePoco);
    }
}
