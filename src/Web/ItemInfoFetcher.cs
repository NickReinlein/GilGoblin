using System.Net.Http.Json;
using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;

namespace GilGoblin.Web;

public class ItemInfoFetcher : DataFetcher<ItemInfoWebPoco, ItemInfoWebResponse>, IItemInfoFetcher
{
    private readonly ILogger<ItemInfoFetcher> _logger;

    public ItemInfoFetcher(ILogger<ItemInfoFetcher> logger)
        : base(ItemInfoBaseUrl)
    {
        _logger = logger;
    }

    public ItemInfoFetcher(HttpClient client, ILogger<ItemInfoFetcher> logger)
        : base(ItemInfoBaseUrl, client)
    {
        _logger = logger;
    }

    public async Task<ItemInfoWebPoco?> FetchItemInfoAsync(int id)
    {
        var idString = $"{id}";
        var pathSuffix = string.Concat(new[] { idString, SelectiveColumnsSingle });

        return await GetAsync(pathSuffix);
    }

    public async Task<IEnumerable<ItemInfoWebPoco?>> FetchMultipleItemInfosAsync(IEnumerable<int> ids)
    {
        if (!ids.Any())
            return Array.Empty<ItemInfoWebPoco>();

        var path = GetPathFromIDs(ids);

        var response = await GetMultipleAsync(path);

        return response is not null ? response.GetContentAsList() : new List<ItemInfoWebPoco>();
    }

    public static string GetPathFromIDs(IEnumerable<int> ids)
    {
        var idString = string.Empty;
        foreach (var id in ids)
        {
            idString += id.ToString();
            if (id != ids.Last())
                idString.Concat(",");
        }
        return string.Concat(new[] { idString, SelectiveColumnsMulti });
    }

    public async Task<List<List<int>>> GetAllIDsAsBatchJobsAsync()
    {
        var allIDs = await GetMarketableItemIDsAsync();
        if (!allIDs.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(ItemInfosPerPage);
        return batcher.SplitIntoBatchJobs(allIDs);
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
            return returnedList.Cast<int>().Where(i => i > 0).ToList();
        }
        catch
        {
            return new List<int>();
        }
    }

    public int ItemInfosPerPage { get; set; } = 100;

    public static readonly string MarketableItemSuffix = "marketable";

    public static readonly string ItemInfoBaseUrl = $$"""https://universalis.app/api/v2/""";
    public static readonly string SelectiveColumnsMulti = $$"""
?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAverageItemInfo%2Citems.currentAverageItemInfoNQ%2Citems.currentAverageItemInfoHQ,items.averageItemInfo%2Citems.averageItemInfoNQ%2Citems.averageItemInfoHQ%2Citems.lastUploadTime
""";

    public static readonly string SelectiveColumnsSingle = $$"""
?listings=0&entries=0&fields=itemID,worldID,currentAverageItemInfo,currentAverageItemInfoNQ,currentAverageItemInfoHQ,averageItemInfo,averageItemInfoNQ,averageItemInfoHQ,lastUploadTime
""";
}
