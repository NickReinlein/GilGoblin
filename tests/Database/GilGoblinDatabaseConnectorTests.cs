using GilGoblin.Database;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseConnectorTests
{
    private GilGoblinDatabaseConnector _databaseConnector;

    [SetUp]
    public void SetUp()
    {
        _databaseConnector = new GilGoblinDatabaseConnector();
        _databaseConnector.ResourceDirectory = DatabaseTests.GetTestDirectory();
    }

    [Test]
    public void GivenAConnector_WhenConnectionSucceeds_ThenAnOpenConnectionIsReturned()
    {
        var connection = _databaseConnector.Connect();

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
        var connection = _databaseConnector.Connect();

        Assert.Multiple(() =>
        {
            Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);
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
        _databaseConnector.Connect();
        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);

        _databaseConnector.Disconnect();

        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen, Is.False);
    }

    [Test]
    public void GivenAConnector_WhenConnectionDoesNotExistAndDisconnectIsCalled_ThenNoExceptionIsThrown()
    {
        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen, Is.False);
        Assert.DoesNotThrow(_databaseConnector.Disconnect);
    }

    [Test]
    public void GivenAConnector_WhenConnectionFails_ThenNullIsReturned()
    {
        _databaseConnector.Disconnect();
        _databaseConnector.ResourceDirectory = "itsAFake";

        var result = _databaseConnector.Connect();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenAConnector_WhenConnectionExists_ThenItIsReturned()
    {
        var connection = _databaseConnector.Connect();
        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);

        var tryAgain = _databaseConnector.Connect();

        Assert.Multiple(() =>
        {
            Assert.That(tryAgain, Is.Not.Null);
            Assert.That(tryAgain, Is.EqualTo(connection));
            Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
        });
    }
}