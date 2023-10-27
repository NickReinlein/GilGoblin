using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Exceptions;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PricePoco, PriceWebPoco>
{
    private readonly IPriceFetcher _fetcher;
    private readonly IPriceRepository<PricePoco> _priceRepository;

    public PriceUpdater(
        IPriceFetcher fetcher,
        IPriceRepository<PricePoco> priceRepository,
        IDataSaver<PricePoco> saver,
        ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
        : base(saver, fetcher, logger)
    {
        _fetcher = fetcher;
        _priceRepository = priceRepository;
    }

    protected override int? GetWorldId()
    {
        return 34; // TODO Add a mechanic to iterate through all worlds
    }

    protected override async Task ConvertToDbFormatAndSave(List<PriceWebPoco> webPocos)
    {
        try
        {
            var success = await Saver.SaveAsync(webPocos.ToPricePocoList());
            if (!success)
                throw new DatabaseException("Saving from DataSaver returned failure");

            Log.Information($"Saved {webPocos.Count} entries for {nameof(PricePoco)}");
        }
        catch (Exception e)
        {
            Log.Error($"Failed to save {webPocos.Count} entries for {nameof(PricePoco)}: {e.Message}");
        }
    }

    protected override async Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId)
    {
        return await _fetcher.GetIdsAsBatchJobsAsync();
    }
}