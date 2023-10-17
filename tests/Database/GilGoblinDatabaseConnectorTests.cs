// using GilGoblin.Database;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.Database;
//
// public class GilGoblinDatabaseConnectorTests
// {
//     private IConfiguration _configuration;
//     private GilGoblinDatabaseConnector _databaseConnector;
//     private ILogger<GilGoblinDatabaseConnector> _logger;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _configuration = Substitute.For<IConfiguration>();
//         var dbString = "Host=localhost;Port=5432;Database=gilgoblin;Username=gilgoblin;Password=gilgoblin;";
//         _configuration.GetConnectionString(nameof(GilGoblinDbContext)).Returns(dbString);
//         _logger = Substitute.For<ILogger<GilGoblinDatabaseConnector>>();
//         _databaseConnector = new GilGoblinDatabaseConnector(_configuration, _logger);
//     }
//
//     [Test]
//     public void WhenConnectionSucceeds_ThenAnOpenConnectionIsReturned()
//     {
//         var connection = _databaseConnector.Connect();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(connection, Is.Not.Null);
//             Assert.That(connection.State, Is.EqualTo(System.Data.ConnectionState.Open));
//             Assert.That(
//                 connection.DataSource,
//                 Does.Contain("gilgoblin")
//             );
//         });
//     }
//
//     [Test]
//     public void WhenConnectionSucceeds_ThenTheConnectionIsStored()
//     {
//         var connection = _databaseConnector.Connect();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);
//             Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
//             Assert.That(
//                 connection?.DataSource,
//                 Does.Contain("gilgoblin")
//             );
//         });
//     }
//
//     [Test]
//     public void WhenConnectionExists_ThenItIsReturned()
//     {
//         var connection = _databaseConnector.Connect();
//         Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);
//
//         var tryAgain = _databaseConnector.Connect();
//
//         Assert.Multiple(() =>
//         {
//             Assert.That(tryAgain, Is.Not.Null);
//             Assert.That(tryAgain, Is.EqualTo(connection));
//             Assert.That(GilGoblinDatabaseConnector.Connection, Is.EqualTo(connection));
//         });
//     }
//
//     [Test]
//     public void WhenConnectionExists_ThenDisconnectClosesTheConnection()
//     {
//         _databaseConnector.Connect();
//         Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen);
//
//         _databaseConnector.Disconnect();
//
//         Assert.That(GilGoblinDatabaseConnector.IsConnectionOpen, Is.False);
//     }
//
//     [Test]
//     public void WhenConnectionDoesNotExistAndDisconnectIsCalled_ThenNoExceptionIsThrown()
//     {
//         if (GilGoblinDatabaseConnector.IsConnectionOpen)
//             _databaseConnector.Disconnect();
//         Assert.That(!GilGoblinDatabaseConnector.IsConnectionOpen);
//
//         Assert.DoesNotThrow(_databaseConnector.Disconnect);
//     }
// }