using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Batcher;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Fetcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DbUpdateException = System.Data.Entity.Infrastructure.DbUpdateException;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PricePoco, PriceWebPoco>
{
    private List<int> _allItemIds;
    private const int dataExpiryInHours = 48;

    public PriceUpdater(
        IServiceScopeFactory scopeFactory,
        ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
        : base(scopeFactory, logger)
    {
    }

    protected override int? GetWorldId()
    {
        return 34; // TODO Add a mechanic to iterate through all worlds
    }

    protected override async Task FetchUpdatesAsync(int? worldId, List<int> idList, CancellationToken ct)
    {
        using var scope = ScopeFactory.CreateScope();
        var fetcher = scope.ServiceProvider.GetRequiredService<IPriceFetcher>();
        var batcher = new Batcher<int>(fetcher.GetEntriesPerPage());
        var batches = batcher.SplitIntoBatchJobs(idList);

        while (!ct.IsCancellationRequested)
        {
            foreach (var batch in batches)
            {
                var fetched = await fetcher.FetchByIdsAsync(ct, batch, worldId);
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
            using var scope = ScopeFactory.CreateScope();
            var saver = scope.ServiceProvider.GetRequiredService<IDataSaver<PricePoco>>();
            var success = await saver.SaveAsync(webPocos.ToPricePocoList());
            if (!success)
                throw new DbUpdateException("Saving from DataSaver returned failure");
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to save {webPocos.Count} entries for {nameof(PricePoco)}: {e.Message}");
        }
    }

    protected override async Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        try
        {
            if (worldId is null or < 1)
                throw new Exception("World Id cannot be null");

            using var scope = ScopeFactory.CreateScope();
            var gilGoblinDbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var marketableIdsFetcher = scope.ServiceProvider.GetRequiredService<IMarketableItemIdsFetcher>();

            if (_allItemIds is null || !_allItemIds.Any())
            {
                _allItemIds = new List<int>();
                var marketableItemIdList = await marketableIdsFetcher.GetMarketableItemIdsAsync();

                var recipePocos = gilGoblinDbContext.Recipe.Cast<RecipePoco>().ToList();
                var ingredientItemIds =
                    recipePocos
                        .SelectMany(r =>
                            r.GetActiveIngredients()
                                .Select(i => i.ItemId))
                        .Distinct()
                        .ToList();

                _allItemIds =
                    marketableItemIdList
                        .Concat(ingredientItemIds)
                        .ToHashSet()
                        .ToList();
            }

            if (!_allItemIds.Any())
                throw new WebException("Failed to fetch Marketable Item Ids");

            var world = worldId.GetValueOrDefault();
            var currentPrices = gilGoblinDbContext.Price.Where(cp => cp.WorldId == world).ToList();
            var currentPriceIds = currentPrices.Select(c => c.GetId()).ToList();
            var newPriceIds = _allItemIds.Except(currentPriceIds).ToList();

            var outdatedPrices = currentPrices.Where(p =>
            {
                var timestamp = new DateTime(p.LastUploadTime).ToUniversalTime();
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