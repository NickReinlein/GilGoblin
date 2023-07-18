using GilGoblin.Database;
using GilGoblin.Web;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GoblinDatabaseTests
{
    private GoblinDatabase _db;
    private IPriceDataFetcher _priceFetcher;

    [SetUp]
    public void SetUp()
    {
        _priceFetcher = Substitute.For<IPriceDataFetcher>();
        _db = new GoblinDatabase(_priceFetcher);
    }

    [Test]
    public async Task GivenGetContextAsyncIsCalled_WhenConnectionFails_ThenNullIsReturned()
    {
        var response = await _db.GetContextAsync();

        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task GivenGetContextAsyncIsCalled_WhenConnectionSucceeds_ThenContextIsReturned()
    {
        var response = await _db.GetContextAsync();

        Assert.That(response, Is.TypeOf<GilGoblinDbContext>());
    }
}
