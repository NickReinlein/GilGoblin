using GilGoblin.Controllers;
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
    private IItemRepository _repo;
    public static readonly int WorldID = 34;

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IItemRepository>();
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
    public async Task GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfItemsIsReturned()
    {
        var poco1 = CreatePoco();
        var poco2 = CreatePoco();
        poco2.ID = poco1.ID + 100;
        _repo.GetAll().Returns(new List<ItemInfoPoco>() { poco1, poco2 });

        var result = await _controller.GetAll();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Count(i => i.ID == poco1.ID), Is.EqualTo(1));
        Assert.That(result.Count(i => i.ID == poco2.ID), Is.EqualTo(1));
    }

    [Test]
    public async Task GivenAController_WhenWeReceiveAGetRequest_ThenAItemIsReturned()
    {
        var poco1 = CreatePoco();
        _repo.Get(poco1.ID).Returns(poco1);

        var result = await _controller.Get(poco1.ID);

        Assert.That(result, Is.EqualTo(poco1));
    }

    [Test]
    public async Task GivenAController_WhenWeReceiveAGetRequestForANonExistentItem_ThenNullIsReturned()
    {
        var result = await _controller.Get(42);

        Assert.That(result, Is.Null);
    }

    private static ItemInfoPoco CreatePoco() =>
        new()
        {
            ID = 200,
            CanBeHq = true,
            IconID = 2332,
            Description = "testDesc",
            Level = 83,
            Name = "testItem",
            StackSize = 1,
            VendorPrice = 3222
        };
}
