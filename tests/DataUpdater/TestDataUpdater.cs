using GilGoblin.Database;
using GilGoblin.DataUpdater;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Tests.DataUpdater;

public partial class DataUpdaterTests
{
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