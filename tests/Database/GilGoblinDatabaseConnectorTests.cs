using GilGoblin.Database;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseConnectorTests
{
    private string _testDirectory;

    [SetUp]
    public void SetUp() { }

    [Test]
    public void GivenAConnector_WhenConnectionSucceeds_ThenConnectionIsReturned()
    {
        var result = GilGoblinDatabaseConnector.Connect();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GivenAConnector_WhenConnectionFails_ThenNullIsReturned()
    {
        var result = GilGoblinDatabaseConnector.Connect("itsAFake");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAConnector_WhenConnectionExists_ThenItIsReturned()
    {
        var connection = GilGoblinDatabaseConnector.Connect();

        var tryAgain = GilGoblinDatabaseConnector.Connect();

        Assert.That(tryAgain, Is.EqualTo(connection));
    }
}
