using System.Linq;
using GilGoblin.Api.Controllers;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Controllers;

public class PriceControllerTests
{
    private PriceController _controller;
    private IPriceRepository<PricePoco> _repo;

    public const int worldId = 34;

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IPriceRepository<PricePoco>>();
        _controller = new PriceController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<PriceController>()
        );
    }

    [Test]
    public void GivenAPriceController_WhenWeSetup_ThenItIsCreatedSuccessfully()
    {
        Assert.That(_controller, Is.Not.Null);
    }

    [Test]
    public void GivenAPriceController_WhenWeReceiveAGetAllRequest_ThenAListOfPricesIsReturned()
    {
        var poco1 = GetPricePoco();
        var poco2 = GetPricePoco() with { ItemId = poco1.ItemId + 100 };
        _repo.GetAll(worldId).Returns([poco1, poco2]);

        var result = _controller.GetAll(worldId);

        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Count(i => i.ItemId == poco1.ItemId), Is.EqualTo(1));
            Assert.That(result.Count(i => i.ItemId == poco2.ItemId), Is.EqualTo(1));
        });
    }

    [Test]
    public void GivenAPriceController_WhenWeReceiveAGetRequest_ThenAPriceIsReturned()
    {
        var price = GetPricePoco();
        _repo.Get(worldId, price.ItemId, price.IsHq).Returns(price);

        var result = _controller.Get(worldId, price.ItemId, true);

        Assert.That(result, Is.EqualTo(price));
    }

    [Test]
    public void GivenAPriceController_WhenWeReceiveAGetRequestForAnInvalidPrice_ThenNullIsReturned()
    {
        var result = _controller.Get(worldId, 42, true);

        Assert.That(result, Is.Null);
    }

    private static PricePoco GetPricePoco() => new(1, worldId, true);
}