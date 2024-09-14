using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Api.Repository;
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
    ILogger<DataUpdater<PricePoco, PriceWebPoco>> logger)
    : DataUpdater<PricePoco, PriceWebPoco>(scopeFactory, logger)
{
    private List<int> AllItemIds { get; set; }
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
        var updateList = ConvertWebToDbFormat(webPocos);

        if (!updateList.Any())
            return;

        try
        {
            using var scope = ScopeFactory.CreateScope();
            var saver = scope.ServiceProvider.GetRequiredService<IDataSaver<PricePoco>>();
            var success = await saver.SaveAsync(updateList);
            if (!success)
                throw new DbUpdateException($"Saving from {nameof(IDataSaver<PricePoco>)} returned failure");
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to save {webPocos.Count} entries for {nameof(PriceWebPoco)}: {e.Message}");
        }
    }

    private static List<PricePoco> ConvertWebToDbFormat(List<PriceWebPoco> webPocos)
    {
        return webPocos
            .SelectMany(x => new List<PricePoco>
            {
                new() { ItemId = x.ItemId, Updated = DateTimeOffset.Now, IsHq = true },
                new() { ItemId = x.ItemId, Updated = DateTimeOffset.Now, IsHq = false }
            })
            .ToList();
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
        using var scope = ScopeFactory.CreateScope();
        var marketableIdsFetcher = scope.ServiceProvider.GetRequiredService<IMarketableItemIdsFetcher>();
        var recipeRepo = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();
        AllItemIds = [];

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

    private async Task AwaitDelay(CancellationToken ct)
    {
        var delay = GetApiSpamDelayInMs();
        Logger.LogInformation($"Awaiting delay of {delay}ms before next batch call (Spam prevention)");
        await Task.Delay(delay, ct);
    }
}