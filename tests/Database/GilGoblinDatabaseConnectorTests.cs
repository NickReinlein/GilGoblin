using GilGoblin.Database;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseConnectorTests
{
    private GilGoblinDatabaseConnector _databaseConnector;
    private ILogger<GilGoblinDatabaseConnector> _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<GilGoblinDatabaseConnector>>();
        _databaseConnector = new GilGoblinDatabaseConnector(_logger);
    }

    [Test]
    public void WhenConnectionSucceeds_ThenAnOpenConnectionIsReturned()
    {
        var connection = _databaseConnector.Connect();

        Assert.Multiple(() =>
        {
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.State, Is.EqualTo(System.Data.ConnectionState.Open));
            Assert.That(
                connection.DataSource,
                Does.Contain(GilGoblinDatabaseConnector._dbFileName)
            );
        });
    }

    [Test]
    public void WhenConnectionSucceeds_ThenTheConnectionIsStored()
    {
        var connection = _databaseConnector.Connect();

        Assert.Multiple(() =>
        {
            Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);
            Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
            Assert.That(
                connection?.DataSource,
                Does.Contain(GilGoblinDatabaseConnector._dbFileName)
            );
        });
    }

    [Test]
    public void WhenConnectionExists_ThenItIsReturned()
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

    [Test]
    public void WhenConnectionExists_ThenDisconnectClosesTheConnection()
    {
        _databaseConnector.Connect();
        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);

        _databaseConnector.Disconnect();

        Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen, Is.False);
    }

    [Test]
    public void WhenConnectionDoesNotExistAndDisconnectIsCalled_ThenNoExceptionIsThrown()
    {
        Assert.That(!GilGoblinDatabaseConnector.IsConnectionOpen);

        Assert.DoesNotThrow(_databaseConnector.Disconnect);
    }

    [Test]
    public void WhenGetDatabasePath_ThenACombinedPathWithDbNameIsReturned()
    {
        var path = _databaseConnector.GetDatabasePath();
        var dbName = GilGoblinDatabaseConnector._dbFileName;

        var result = _databaseConnector.GetDatabasePath();

        Assert.Multiple(() =>
        {
            Assert.That(result.Contains(dbName));
            Assert.That(result.Contains(path));
        });
    }
}

//     private static string ResourcesFolderPath => Path.Combine(GetCurrentDirectory(), "resources/");
//
//     private static string GetCurrentDirectory() =>
//         Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName
//         ?? string.Empty;
// }

// public class GilGoblinDatabaseConnectorTestClass : GilGoblinDatabaseConnector
// {
//     public new static string DbFileName = "GilGoblin.db";
//
//     public GilGoblinDatabaseConnectorTestClass(ILogger<GilGoblinDatabaseConnector> logger)
//         : base(logger)
//     {
//     }
// }