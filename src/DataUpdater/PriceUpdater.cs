using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PriceWebPoco>
{
    private readonly IPriceFetcher _fetcher;

    public PriceUpdater(
        IPriceFetcher fetcher,
        IDataSaver<PriceWebPoco> saver,
        ILogger<DataUpdater<PriceWebPoco>> logger)
        : base(fetcher, saver, logger)
    {
        _fetcher = fetcher;
    }

    protected override Task<List<int>> GetIdsToUpdateAsync()
        => Task.FromResult(_fetcher.GetIdsAsBatchJobsAsync().Result[0]);
}