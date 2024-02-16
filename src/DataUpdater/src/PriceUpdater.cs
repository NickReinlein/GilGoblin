using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Api.Repository;
using GilGoblin.Batcher;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Fetcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class PriceUpdater : DataUpdater<PricePoco, PriceWebPoco>
{
    public List<int> AllItemIds { get; private set; }
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
                try
                {
                    var fetched = await fetcher.FetchByIdsAsync(ct, batch, worldId);
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
        var updateList = webPocos.ToPricePocoList();
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
            Logger.LogError($"Failed to save {webPocos.Count} entries for {nameof(PricePoco)}: {e.Message}");
        }
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
                var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(p.LastUploadTime).UtcDateTime;
                var ageInHours = (DateTimeOffset.UtcNow - timestamp).TotalHours;
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
        AllItemIds = new List<int>();

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