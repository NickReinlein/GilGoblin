using GilGoblin.Database;
using GilGoblin.DataUpdater;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class DataUpdaterTests : InMemoryTestDb
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

        _dbContext = new GilGoblinDbContext(_options, _configuration);
        _fetcher = Substitute.For<IDataFetcher<House, HouseResponse>>();
        _logger = Substitute.For<ILogger<TestDataUpdater>>();
        _housesToUpdate = new List<House>();
        _urlToUse = "www.baseUrl.com";

        _updater = new TestDataUpdater(_dbContext, _fetcher, _logger, _housesToUpdate, _urlToUse);
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreNoEntriesToUpdate_WeDoNotMakeAnyCalls()
    {
        await _updater.UpdateAsync();

        await _fetcher.DidNotReceive().GetMultipleAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdate_WeFetchUpdatesForThoseEntries()
    {
        _housesToUpdate.Add(new House { CivicNumber = 123, StreetName = "Elm Street" });
        _fetcher.GetMultipleAsync(Arg.Is<string>(x => x.Contains(_urlToUse))).Returns(new HouseResponse()
        {
            Houses = _housesToUpdate
        });

        await _updater.UpdateAsync();

        await _fetcher.Received().GetMultipleAsync(Arg.Is<string>(x => x.Contains(_urlToUse)));
    }

    [Test]
    public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdate_WeSaveTheUpdatesForThoseEntries()
    {
        _housesToUpdate.Add(new House { CivicNumber = 123, StreetName = "Elm Street" });
        _fetcher.GetMultipleAsync(Arg.Is<string>(x => x.Contains(_urlToUse))).Returns(new HouseResponse()
        {
            Houses = _housesToUpdate
        });

        await _updater.UpdateAsync();

        using var context = new GilGoblinDbContext(_options, _configuration);
        Assert.That(context.)
    }

    public class House
    {
        public int CivicNumber { get; set; }
        public string StreetName { get; set; }
    }

    public class HouseResponse : IReponseToList<House>
    {
        public List<House> Houses { get; set; }

        public List<House> GetContentAsList() => Houses;
    }

    public class TestDataUpdater : DataUpdater<House, HouseResponse>
    {
        private IEnumerable<House> _housesToUpdate;
        private string _urlToUse;

        public TestDataUpdater(
            GilGoblinDbContext dbContext,
            IDataFetcher<House, HouseResponse> fetcher,
            ILogger<DataUpdater<House, HouseResponse>> logger,
            IEnumerable<House> housesToUpdate,
            string urlToUse) : base(dbContext, fetcher, logger)
        {
            _housesToUpdate = housesToUpdate;
            _urlToUse = urlToUse;
        }

        protected override async Task<IEnumerable<House>> GetEntriesToUpdateAsync() => await Task.Run(() => _housesToUpdate);
        protected override async Task<string> GetUrlPathFromEntries(IEnumerable<House> entries) => await Task.Run(() => _urlToUse);
    }
}