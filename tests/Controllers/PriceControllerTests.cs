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
    public static readonly int WorldID = 34;

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

    [TearDown]
    public void TearDown()
    {
        _repo.ClearReceivedCalls();
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfPricesIsReturned()
    {
        var poco1 = CreatePoco();
        var poco2 = CreatePoco();
        poco2.ItemID = poco1.ItemID + 100;
        _repo.GetAll(WorldID).Returns(new List<PricePoco>() { poco1, poco2 });

        var result = _controller.GetAll(WorldID);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Count(i => i.ItemID == poco1.ItemID), Is.EqualTo(1));
        Assert.That(result.Count(i => i.ItemID == poco2.ItemID), Is.EqualTo(1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenAPriceIsReturned()
    {
        var poco1 = CreatePoco();
        _repo.Get(WorldID, poco1.ItemID).Returns(poco1);

        var result = _controller.Get(WorldID, poco1.ItemID);

        Assert.That(result, Is.EqualTo(poco1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequestForANonExistentPrice_ThenNullIsReturned()
    {
        var result = _controller.Get(WorldID, 42);

        Assert.That(result, Is.Null);
    }

    private static PricePoco CreatePoco() =>
        new()
        {
            ItemID = 200,
            AverageSold = 333,
            AverageListingPrice = 555,
            WorldID = 34
        };
}