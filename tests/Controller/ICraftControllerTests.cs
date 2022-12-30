using GilGoblin.Controller;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class ICraftControllerTests<T> where T : class
{
    private readonly ICraftController<T> _controller = Substitute.For<ICraftController<T>>();

    [SetUp]
    public void SetUp() { }

    [TearDown]
    public void TearDown()
    {
        _controller.ClearReceivedCalls();
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetBestCraftsRequest_ThenAnEnumerableResultIsReturned()
    {
        var result = _controller.GetBestCrafts(34);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<IEnumerable<T>>());
        Assert.That(result.Count, Is.GreaterThan(1));
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenOneResultIsReturned()
    {
        var result = _controller.GetCraft(34, 3);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<T>());
    }
}
