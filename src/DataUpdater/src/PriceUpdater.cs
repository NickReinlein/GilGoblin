using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Converters;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class PriceUpdater(
    IServiceScopeFactory scopeFactory,
    ILogger<PriceUpdater> logger)
    : DataUpdater<PricePoco, PriceWebPoco>(scopeFactory, logger)
{
    private List<int> AllItemIds { get; set; } = [];
    private DateTimeOffset LastUpdated { get; set; }
    private const int dataExpiryInHours = 48;

    protected override async Task ExecuteUpdateAsync(CancellationToken ct)
    {
        var worlds = GetWorlds();
        var fetchTasks = new List<Task>();

        foreach (var world in worlds)
        {
            Logger.LogInformation("Fetching price updates for world id/name: {Id}/{Name}", world.Id, world.Name);
            fetchTasks.Add(FetchAsync(world.Id, ct));
        }

        await Task.WhenAll(fetchTasks);
    }


    protected override async Task FetchUpdatesAsync(int? worldId, List<int> idList, CancellationToken ct)
    {
        using var scope = ScopeFactory.CreateScope();
        var fetcher = scope.ServiceProvider.GetRequiredService<IPriceFetcher>();
        var batcher = new Batcher.Batcher<int>(fetcher.GetEntriesPerPage());
        var batches = batcher.SplitIntoBatchJobs(idList);

        while (!ct.IsCancellationRequested)
        {
            foreach (var batch in batches)
            {
                try
                {
                    var fetched = await fetcher.FetchByIdsAsync(batch, worldId, ct);
                    if (!fetched.Any())
                    {
                        Logger.LogDebug(
                            "Fetched {Count} items for world id {WorldId} but received no price data in response",
                            batch.Count,
                            worldId);
                        continue;
                    }

                    await ConvertAndSaveToDbAsync(fetched);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Failed to get batch: {e.Message}");
                }

                try
                {
                    await AwaitDelay(ct);
                }
                catch (TaskCanceledException)
                {
                    const string message =
                        $"The cancellation token was cancelled. Ending service {nameof(PriceUpdater)}";
                    Logger.LogInformation(message);
                }
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
            var converter = scope.ServiceProvider.GetRequiredService<IPriceConverter>();

            var updateList = await ConvertWebToDbFormatAsync(webPocos, converter);
            var filteredList = updateList
                .Where(u => u.AverageSalePriceId > 0 || u.RecentPurchaseId > 0 || u.MinListingId > 0).ToList();
            if (!filteredList.Any())
                return;

            var success = await saver.SaveAsync(filteredList);
            if (!success)
                throw new DbUpdateException($"Saving from {nameof(IDataSaver<PricePoco>)} returned failure");
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to save {Count} entries for {TypeString}", webPocos.Count, nameof(PricePoco));
        }
    }

    private async Task<List<PricePoco>> ConvertWebToDbFormatAsync(List<PriceWebPoco> webPocos,
        IPriceConverter converter)
    {
        var updateList = new List<PricePoco>();
        foreach (var webPoco in webPocos)
        {
            try
            {
                var worldId = webPoco.GetWorldId();
                if (worldId == 0)
                {
                    logger.LogDebug("Skipping price for {ItemId} with no world id", webPoco.ItemId);
                    continue;
                }

                var (hq, nq) = await converter.ConvertAsync(webPoco, worldId);
                if (hq is not null)
                    updateList.Add(hq);
                if (nq is not null)
                    updateList.Add(nq);
            }
            catch (Exception e)
            {
                logger.LogDebug(e, $"Failed to convert {nameof(PriceWebPoco)} to db format");
            }
        }

        return updateList;
    }

    protected override List<WorldPoco> GetWorlds()
    {
        using var scope = ScopeFactory.CreateScope();
        var worldRepo = scope.ServiceProvider.GetRequiredService<IWorldRepository>();
        return worldRepo.GetAll().ToList();
    }

    protected override async Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        try
        {
            if (worldId is null or < 1)
                throw new Exception("World Id is invalid");

            var world = worldId.GetValueOrDefault();
            if (world <= 0)
                throw new ArgumentException($"Interpreted world id as {world}");

            await FillItemIdCache();

            using var scope = ScopeFactory.CreateScope();
            var priceRepo = scope.ServiceProvider.GetRequiredService<IPriceRepository<PricePoco>>();
            var currentPrices = priceRepo.GetAll(world).ToList();
            var currentPriceIds = currentPrices.Select(c => c.GetId()).ToList();
            var newPriceIds = AllItemIds.Except(currentPriceIds).ToList();

            var outdatedPrices = currentPrices.Where(p =>
            {
                var priceAge = p.Updated;
                var ageInHours = (DateTimeOffset.UtcNow - priceAge).TotalHours;
                return ageInHours > dataExpiryInHours;
            }).ToList();

            var outdatedPriceIdList = outdatedPrices.Select(o => o.GetId());
            var idsToUpdate = outdatedPriceIdList.Concat(newPriceIds).ToList();

            return idsToUpdate;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to get the Ids to update for world {worldId}: {e.Message}");
            return new List<int>();
        }
    }

    private async Task FillItemIdCache()
    {
        if (AllItemIds.Any() && (DateTimeOffset.UtcNow - LastUpdated).TotalHours < 48)
            return;

        try
        {
            using var scope = ScopeFactory.CreateScope();
            var marketableIdsFetcher = scope.ServiceProvider.GetRequiredService<IMarketableItemIdsFetcher>();
            var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            AllItemIds.Clear();
            LastUpdated = DateTimeOffset.UtcNow;

            var recipes = recipeRepo.GetAll().ToList();
            Logger.LogDebug($"Received {recipes.Count} recipes  from recipe repository");
            var ingredientItemIds =
                recipes
                    .SelectMany(r =>
                        r.GetActiveIngredients()
                            .Select(i => i.ItemId))
                    .Distinct()
                    .ToList();

            var marketableItemIdList = await marketableIdsFetcher.GetMarketableItemIdsAsync();
            Logger.LogDebug(
                $"Received {marketableItemIdList.Count} marketable item Ids from ${typeof(MarketableItemIdsFetcher)}");
            if (!marketableItemIdList.Any())
                throw new WebException("Failed to fetch marketable item ids");

            AllItemIds =
                marketableItemIdList
                    .Concat(ingredientItemIds)
                    .Distinct()
                    .ToList();
            Logger.LogDebug($"Found {AllItemIds.Count} total item Ids for ${typeof(PriceUpdater)}");
        }
        catch (Exception e)
        {
            Logger.LogWarning(e, "Failed to fill item Id cache");
            Logger.LogWarning(e, "Failed to fill item Id cache");
        }
    }

    private async Task AwaitDelay(CancellationToken ct)
    {
        var delay = GetApiSpamDelayInMs();
        Logger.LogInformation($"Awaiting delay of {delay}ms before next batch call (Spam prevention)");
        await Task.Delay(delay, ct);
    }
}