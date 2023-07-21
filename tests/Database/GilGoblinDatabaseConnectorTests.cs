using GilGoblin.Database;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseConnectorTests
{
    private string _testDirectory;

    [SetUp]
    public void SetUp()
    {
        _testDirectory = DatabaseTests.GetTestDirectory();
    }

    [Test]
    public void GivenAConnector_WhenConnectionSucceeds_ThenAnOpenConnectionIsReturned()
    {
        var connection = GilGoblinDatabaseConnector.Connect(_testDirectory);

        Assert.Multiple(() =>
        {
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection?.State, Is.EqualTo(System.Data.ConnectionState.Open));
            Assert.That(
                connection?.DataSource,
                Does.Contain(GilGoblinDatabaseConnector.DbFileName)
            );
        });
    }

    [Test]
    public void GivenAConnector_WhenConnectionSucceeds_ThenTheConnectionIsStored()
    {
        var connection = GilGoblinDatabaseConnector.Connect(_testDirectory);

        Assert.Multiple(() =>
        {
            Assert.That(GilGoblinDatabaseConnector.ConnectionIsOpen);
            Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
            Assert.That(
                connection?.DataSource,
                Does.Contain(GilGoblinDatabaseConnector.DbFileName)
            );
        });
    }

    [Test]
    public void GivenAConnector_WhenConnectionExists_ThenDisconnectClosesTheConnection()
    {
        GilGoblinDatabaseConnector.Connect(_testDirectory);
        Assert.That(GilGoblinDatabaseConnector.ConnectionIsOpen);

        GilGoblinDatabaseConnector.Disconnect();

        Assert.That(GilGoblinDatabaseConnector.ConnectionIsOpen, Is.False);
    }

    [Test]
    public void GivenAConnector_WhenConnectionDoesNotExistAndDisconnectIsCalled_ThenNoExceptionIsThrown()
    {
        Assert.That(GilGoblinDatabaseConnector.ConnectionIsOpen, Is.False);
        Assert.DoesNotThrow(GilGoblinDatabaseConnector.Disconnect);
    }

    [Test]
    public void GivenAConnector_WhenConnectionFails_ThenNullIsReturned()
    {
        GilGoblinDatabaseConnector.Disconnect();
        var result = GilGoblinDatabaseConnector.Connect("itsAFake");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAConnector_WhenConnectionExists_ThenItIsReturned()
    {
        var connection = GilGoblinDatabaseConnector.Connect(_testDirectory);
        Assert.That(GilGoblinDatabaseConnector.ConnectionIsOpen);

        var tryAgain = GilGoblinDatabaseConnector.Connect();

        Assert.Multiple(() =>
        {
            Assert.That(tryAgain, Is.Not.Null);
            Assert.That(tryAgain, Is.EqualTo(connection));
            Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
        });
    }
}
