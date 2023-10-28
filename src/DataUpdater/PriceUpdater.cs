using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Exceptions;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using GilGoblin.Repository;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PricePoco, PriceWebPoco>
{
    private readonly IPriceRepository<PricePoco> _priceRepository;
    private readonly IPriceFetcher _priceFetcher;
    private readonly IMarketableItemIdsFetcher _marketableIdsFetcher;
    private List<int> _allItemIds;
    private const int dataExpiryInHours = 24;

    public PriceUpdater(
        IPriceFetcher priceFetcher,
        IMarketableItemIdsFetcher marketableIdsFetcher,
        IPriceRepository<PricePoco> priceRepository,
        IDataSaver<PricePoco> saver,
        ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
        : base(saver, priceFetcher, logger)
    {
        _priceFetcher = priceFetcher;
        _marketableIdsFetcher = marketableIdsFetcher;
        _priceRepository = priceRepository;
    }

    protected override int? GetWorldId()
    {
        return 34; // TODO Add a mechanic to iterate through all worlds
    }

    protected override async Task FetchUpdatesAsync(CancellationToken ct, int? worldId, List<int> idList)
    {
        var batcher = new Batcher<int>(_priceFetcher.GetEntriesPerPage());
        var batches = batcher.SplitIntoBatchJobs(idList);

        while (!ct.IsCancellationRequested)
        {
            foreach (var batch in batches)
            {
                var fetched = await _priceFetcher.FetchByIdsAsync(ct, batch, worldId);
                if (fetched.Any())
                    await ConvertAndSaveToDbAsync(fetched);

                await AwaitDelay(ct);
            }

            return;
        }
    }


    protected override async Task ConvertAndSaveToDbAsync(List<PriceWebPoco> webPocos)
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

    protected override async Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        try
        {
            if (worldId is null or < 1)
                throw new Exception("World Id cannot be null");

            _allItemIds ??= await _marketableIdsFetcher.GetMarketableItemIdsAsync();
            if (!_allItemIds.Any())
                throw new WebException("Failed to fetch Marketable Item Ids");

            var world = worldId.GetValueOrDefault();
            var currentPrices = _priceRepository.GetAll(world).ToList();
            var currentPriceIds = currentPrices.Select(c => c.GetId()).ToList();
            var newPriceIds = _allItemIds.Except(currentPriceIds).ToList();

            var outdatedPrices = currentPrices.Where(p =>
            {
                var timestamp = p.LastUploadTime.ConvertLongUnixMsToDateTime().ToUniversalTime();
                var ageInHours = (DateTimeOffset.UtcNow - timestamp).Hours;
                return ageInHours > dataExpiryInHours;
            }).ToList();
            var outdatedPriceIdList = outdatedPrices.Select(o => o.GetId());

            return outdatedPriceIdList.Concat(newPriceIds).ToList();
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
            return new List<int>();
        }
    }

    private async Task AwaitDelay(CancellationToken ct)
    {
        var delay = GetApiSpamDelayInMs();
        Logger.LogDebug($"Awaiting delay of {delay}ms before next batch call (Spam prevention)");
        await Task.Delay(delay, ct);
    }
}
