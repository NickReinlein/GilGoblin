using GilGoblin.Controller;
using GilGoblin.Pocos;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class DataControllerTests
{
    private readonly IDataController<ItemInfoPoco> _controller = Substitute.For<
        IDataController<ItemInfoPoco>
    >();

    [SetUp]
    public void SetUp() { }

    [TearDown]
    public void TearDown()
    {
        _controller.ClearReceivedCalls();
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAnEnumerableResultIsReturned()
    {
        var result = _controller.GetAll();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<IEnumerable<ItemInfoPoco>>());
        Assert.That(result.Count, Is.GreaterThan(1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenOneResultIsReturned()
    {
        var result = _controller.Get(3);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<ItemInfoPoco>());
    }
}
