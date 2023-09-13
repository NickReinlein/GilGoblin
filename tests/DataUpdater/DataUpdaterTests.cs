using GilGoblin.Database;
using GilGoblin.Web;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public partial class DataUpdaterTests : InMemoryTestDb
{
    private TestDataUpdater _updater;
    private GilGoblinDbContext _dbContext;
    private IDataFetcher<House, HouseResponse> _fetcher;
    private ILogger<TestDataUpdater> _logger;
    private List<House> _housesToUpdate;
    private string _urlToUse;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _housesToUpdate = new List<House> { new() { CivicNumber = 123 } };
        _urlToUse = "www.baseurl.com";

        _dbContext = new GilGoblinDbContext(_options, _configuration);
        _fetcher = Substitute.For<IDataFetcher<House, HouseResponse>>();
        _fetcher.GetMultipleAsync(Arg.Is<string>(x => x.Contains(_urlToUse))).Returns(new HouseResponse()
        {
            Houses = _housesToUpdate
        });
        _logger = Substitute.For<ILogger<TestDataUpdater>>();

        _updater = new TestDataUpdater(_dbContext, _fetcher, _logger, _housesToUpdate, _urlToUse);
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreNoEntriesToUpdate_ThenWeDoNotMakeAnyCalls()
    {
        _housesToUpdate = new List<House>();
        _fetcher = Substitute.For<IDataFetcher<House, HouseResponse>>();
        _updater = new TestDataUpdater(_dbContext, _fetcher, _logger, _housesToUpdate, _urlToUse);

        await _updater.UpdateAsync();

        await _fetcher.DidNotReceive().GetMultipleAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreNoEntriesToUpdate()
    {
        _housesToUpdate = new List<House>();
        _fetcher = Substitute.For<IDataFetcher<House, HouseResponse>>();
        _updater = new TestDataUpdater(_dbContext, _fetcher, _logger, _housesToUpdate, _urlToUse);

        await _updater.UpdateAsync();

        await _fetcher.DidNotReceive().GetMultipleAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdate_ThenWeFetchUpdatesForThoseEntries()
    {
        await _updater.UpdateAsync();

        await _fetcher.Received().GetMultipleAsync(Arg.Is<string>(x => x.Contains(_urlToUse)));
    }

    [Test]
    public async Task GivenUpdateAsync_WhenUpdatingThrowsAnException_ThenWeCatchAndLogTheError()
    {
        _fetcher.GetMultipleAsync(_urlToUse).Throws<ConnectionAbortedException>();

        Assert.DoesNotThrowAsync(async () => await _updater.UpdateAsync());

        _logger.Received().LogError($"An exception occured during the Api call: The connection was aborted");
    }

    [Test]
    public async Task GivenFetchUpdateForEntries_WhenFetchingReturnsNull_ThenWeDoNotThrowAndLogTheError()
    {
        _fetcher.GetMultipleAsync(_urlToUse).Returns((HouseResponse)null);

        Assert.DoesNotThrowAsync(async () => await _updater.UpdateAsync());

        _logger.Received().LogError($"Failed to fetch the apiUrl: {_urlToUse}");
    }

    public class House
    {
        public int CivicNumber { get; set; }
    }

    public class HouseResponse : IReponseToList<House>
    {
        public List<House> Houses { get; set; }

        public List<House> GetContentAsList() => Houses;
    }
}