using GilGoblin.Controllers;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controllers;

public class ItemControllerTests
{
    private ItemController _controller;
    private IItemRepository _repo;

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

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfItemsIsReturned()
    {
        var poco1 = CreatePoco();
        var poco2 = CreatePoco();
        poco2.Id = poco1.Id + 100;
        _repo.GetAll().Returns(new List<ItemInfoPoco>() { poco1, poco2 });

        var result = _controller.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(i => i.Id == poco1.Id), Is.EqualTo(1));
            Assert.That(result.Count(i => i.Id == poco2.Id), Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenAItemIsReturned()
    {
        var poco1 = CreatePoco();
        _repo.Get(poco1.Id).Returns(poco1);

        var result = _controller.Get(poco1.Id);

        Assert.That(result, Is.EqualTo(poco1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequestForANonExistentItem_ThenNullIsReturned()
    {
        var result = _controller.Get(42);

        Assert.That(result, Is.Null);
    }

    private static ItemInfoPoco CreatePoco() =>
        new()
        {
            Id = 200,
            CanBeHq = true,
            IconId = 2332,
            Description = "testDesc",
            Level = 83,
            Name = "testItem",
            StackSize = 1,
            PriceLow = 322,
            PriceMid = 4222
        };
}
