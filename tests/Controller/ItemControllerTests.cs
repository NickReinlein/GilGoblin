using GilGoblin.Controller;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class ItemControllerTests
{
    private ItemController _controller;
    private readonly IItemRepository _repo = Substitute.For<IItemRepository>();

    private static readonly int _itemId = 108;

    [SetUp]
    public void SetUp()
    {
        _controller = new ItemController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<ItemController>()
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
        _repo.GetAll().Returns(new List<ItemInfoPoco>());

        var result = _controller.GetAll();

        Assert.That(result is IEnumerable<ItemInfoPoco>);
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenOneResultIsReturned()
    {
        _repo.Get(_itemId).Returns(new ItemInfoPoco());

        var result = _controller.Get(_itemId);

        Assert.That(result is ItemInfoPoco);
    }
}
