// using GilGoblin.Database;
// using GilGoblin.Web;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Database;

// public class GoblinDatabaseTests
// {
//     private GilGoblinDatabase _db;
//     private IPriceDataFetcher _priceFetcher;
//     private ISqlLiteDatabaseConnector _dbConnector;

//     [SetUp]
//     public void SetUp()
//     {
//         _dbConnector = Substitute.For<ISqlLiteDatabaseConnector>();

//         _priceFetcher = Substitute.For<IPriceDataFetcher>();

//         _db = new GilGoblinDatabase(_priceFetcher, _dbConnector);
//     }

//     [Test]
//     public async Task GivenGetContextAsyncIsCalled_WhenConnectionFails_ThenNullIsReturned()
//     {
//         var response = await _db.GetContext();

//         Assert.That(response, Is.Null);
//     }

//     // [Test]
//     // public async Task GivenGetContextAsyncIsCalled_WhenConnectionSucceeds_ThenContextIsReturned()
//     // {
//     //     var response = await _db.GetContextAsync();

//     //     Assert.That(response, Is.TypeOf<GilGoblinDbContext>());
//     // }
// }
