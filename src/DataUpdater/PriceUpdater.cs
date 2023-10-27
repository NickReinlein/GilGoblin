using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IItemRepository _itemRepository;
    private readonly IPriceFetcher _fetcher;
    private const int dataExpiryInHours = 24;

    public PriceUpdater(
        IPriceFetcher fetcher,
        IItemRepository itemRepository,
        IPriceRepository<PricePoco> priceRepository,
        IDataSaver<PricePoco> saver,
        ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
        : base(saver, fetcher, logger)
    {
        _fetcher = fetcher;
        _itemRepository = itemRepository;
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

    protected override Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        try
        {
            if (worldId is null or < 1)
                throw new Exception("World Id cannot be null");

            var allItemIds = _itemRepository.GetAll().Select(i => i.GetId()).ToList();

            var world = worldId.GetValueOrDefault();
            var currentPrices = _priceRepository.GetAll(world).ToList();
            var currentPriceIds = currentPrices.Select(c => c.GetId());
            var newPriceIds = allItemIds.Except(currentPriceIds).ToList();

            var outdatedPrices = currentPrices.Where(p =>
            {
                var timestamp = p.LastUploadTime.ConvertLongUnixMsToDateTime().ToUniversalTime();
                var ageInHours = (DateTimeOffset.UtcNow - timestamp).Hours;
                return ageInHours > dataExpiryInHours;
            }).ToList();
            var outdatedPriceIdList = outdatedPrices.Select(o => o.GetId());

            return Task.FromResult(outdatedPriceIdList.Concat(newPriceIds).ToList());
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
            return Task.FromResult(new List<int>());
        }
    }
}