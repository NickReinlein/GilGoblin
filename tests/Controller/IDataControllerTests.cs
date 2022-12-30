using GilGoblin.Controller;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class IDataControllerTests<T> where T : class
{
    private readonly IDataController<T> _controller = Substitute.For<IDataController<T>>();

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
        Assert.That(result, Is.TypeOf<IEnumerable<T>>());
        Assert.That(result.Count, Is.GreaterThan(1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenOneResultIsReturned()
    {
        var result = _controller.Get(3);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<T>());
    }
}
