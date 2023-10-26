using System.Collections.Generic;
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

    protected override int? GetWorldId()
    {
        return 34; // TODO Add a mechanic to iterate through all worlds
    }

    protected override async Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId)
        => await _fetcher.GetIdsAsBatchJobsAsync();
}