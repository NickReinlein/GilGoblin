using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PricePoco, PriceWebPoco>
{
    private readonly IPriceFetcher _fetcher;

    public PriceUpdater(
        IPriceFetcher fetcher,
        IDataSaver<PricePoco> saver,
        ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
        : base(saver, fetcher, logger)
    {
        _fetcher = fetcher;
    }

    protected override int? GetWorldId()
    {
        return 34; // TODO Add a mechanic to iterate through all worlds
    }

    protected override async Task ConvertToDbFormatAndSave(List<PriceWebPoco> webPocos)
        => await Saver.SaveAsync(webPocos.ToPricePocoList());

    protected override async Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId)
        => await _fetcher.GetIdsAsBatchJobsAsync();
}