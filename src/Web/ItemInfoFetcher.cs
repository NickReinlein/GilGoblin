using System;
using System.Net.Http.Json;
using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class ItemInfoFetcher : DataFetcher<ItemInfoWebPoco, ItemInfoWebResponse>, IItemInfoFetcher
{
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<ItemInfoFetcher> _logger;

    public ItemInfoFetcher(HttpClient client, IItemRepository itemRepo, ILogger<ItemInfoFetcher> logger)
        : base(ItemInfoBaseUrl, logger, client)
    {
        _itemRepo = itemRepo;
        _logger = logger;
    }

    protected override string GetUrlPathFromEntries(IEnumerable<int> ids)
    {
        var sb = new StringBuilder();
        var idList = ids.ToList();
        foreach (var id in idList)
        {
            sb.Append(id);
            if (id != idList.Last())
                sb.Append(',');
        }

        return string.Concat(new[] { sb.ToString(), SelectiveColumnsMulti });
    }

    public async Task<List<List<int>>> GetIdsAsBatchJobsAsync()
    {
        var missingIds = await GetAllMissingIds();
        if (!missingIds.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(ItemInfosPerPage);
        return batcher.SplitIntoBatchJobs(missingIds);
    }

    public async Task<List<int>> GetMarketableItemIDsAsync()
    {
        var fullPath = string.Concat(BasePath, MarketableItemSuffix);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return new List<int>();

        try
        {
            var returnedList = await response.Content.ReadFromJsonAsync<List<int>>();
            return returnedList.Where(i => i > 0).ToList();
        }
        catch (Exception e)
        {
            var message =
                $"There was an error getting all marketable item Ids with message {e.Message} with path: {fullPath}";
            _logger.LogError(message);
            return new List<int>();
        }
    }

    private async Task<List<int>> GetAllMissingIds()
    {
        var allIds = await GetMarketableItemIDsAsync();
        if (!allIds.Any())
            return new List<int>();

        var existingIDs = _itemRepo.GetAll().Select(i => i.ID).ToList();
        return allIds.Except(existingIDs).ToList();
    }

    public int ItemInfosPerPage { get; set; } = 100;

    public static readonly string MarketableItemSuffix = "marketable";

    public static readonly string ItemInfoBaseUrl = "https://universalis.app/api/v2/";

    public static readonly string SelectiveColumnsMulti =
        "?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAverageItemInfo%2Citems.currentAverageItemInfoNQ%2Citems.currentAverageItemInfoHQ,items.averageItemInfo%2Citems.averageItemInfoNQ%2Citems.averageItemInfoHQ%2Citems.lastUploadTime";
}