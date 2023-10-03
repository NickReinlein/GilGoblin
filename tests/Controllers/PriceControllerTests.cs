using GilGoblin.Controllers;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controllers;

public class PriceControllerTests
{
    private PriceController _controller;
    private IPriceRepository<PricePoco> _repo;
    public static readonly int WorldId = 34;

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IPriceRepository<PricePoco>>();
        _controller = new PriceController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<PriceController>()
        );
        Assert.That(_controller, Is.Not.Null);
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfPricesIsReturned()
    {
        var poco1 = CreatePoco();
        var poco2 = CreatePoco();
        poco2.ItemId = poco1.ItemId + 100;
        _repo.GetAll(WorldId).Returns(new List<PricePoco>() { poco1, poco2 });

        var result = _controller.GetAll(WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(i => i.ItemId == poco1.ItemId), Is.EqualTo(1));
            Assert.That(result.Count(i => i.ItemId == poco2.ItemId), Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenAPriceIsReturned()
    {
        var poco1 = CreatePoco();
        _repo.Get(WorldId, poco1.ItemId).Returns(poco1);

        var result = _controller.Get(WorldId, poco1.ItemId);

        Assert.That(result, Is.EqualTo(poco1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequestForANonExistentPrice_ThenNullIsReturned()
    {
        var result = _controller.Get(WorldId, 42);

        Assert.That(result, Is.Null);
    }

    private static PricePoco CreatePoco() =>
        new()
        {
            ItemId = 200,
            AverageSold = 333,
            AverageListingPrice = 555,
            WorldId = 34
        };
}
