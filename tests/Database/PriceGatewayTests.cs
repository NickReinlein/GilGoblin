using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class PriceGatewayTests
{
    private PriceGateway _priceGateway;
    private IPriceRepository<PricePoco> _prices;

    [SetUp]
    public void SetUp()
    {
        _prices = Substitute.For<IPriceRepository<PricePoco>>();

        _priceGateway = new PriceGateway(_prices);
    }

    [Test]
    public async Task GivenAGet_ThenTheRepositoryGetIsCalled()
    {
        await _priceGateway.Get(1, 2);

        await _prices.Received(1).Get(1, 2);
    }

    [Test]
    public async Task GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
    {
        var multiple = Enumerable.Range(1, 10);
        await _priceGateway.GetMultiple(22, multiple);

        await _prices.Received(1).GetMultiple(22, multiple);
    }

    [Test]
    public async Task GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
    {
        await _priceGateway.GetAll(33);

        await _prices.Received(1).GetAll(33);
    }
}
