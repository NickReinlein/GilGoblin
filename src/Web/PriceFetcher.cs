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

public class PriceFetcher : DataFetcher<PriceWebPoco, PriceWebResponse>, IPriceDataFetcher
{
    private readonly ILogger<PriceFetcher> _logger;

    public PriceFetcher(ILogger<PriceFetcher> logger)
        : base(PriceBaseUrl)
    {
        _logger = logger;
    }

    public PriceFetcher(HttpClient client, ILogger<PriceFetcher> logger)
        : base(PriceBaseUrl, client)
    {
        _logger = logger;
    }

    public async Task<PriceWebPoco?> FetchPriceAsync(int worldID, int id)
    {
        var worldString = GetWorldString(worldID);
        var idString = $"{id}";
        var pathSuffix = string.Concat(new[] { worldString, idString, SelectiveColumnsSingle });

        return await base.GetAsync(pathSuffix);
    }

    public async Task<IEnumerable<PriceWebPoco?>> FetchMultiplePricesAsync(
        int worldID,
        IEnumerable<int> ids
    )
    {
        var idString = string.Empty;
        var worldString = GetWorldString(worldID);
        if (!ids.Any())
            return Array.Empty<PriceWebPoco>();

        foreach (var id in ids)
        {
            idString += id.ToString();
            if (id != ids.Last())
                idString += ",";
        }
        var pathSuffix = string.Concat(new[] { worldString, idString, SelectiveColumnsMulti });

        var response = await base.GetMultipleAsync(pathSuffix);

        return response is not null ? response.GetContentAsList() : new List<PriceWebPoco>();
    }

    public async Task<List<List<int>>> GetAllIDsAsBatchJobsAsync(int worldID)
    {
        var allIDs = await GetMarketableItemIDsAsync();
        if (!allIDs.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(PricesPerPage);
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

    public static string GetWorldString(int worldID) => $"{worldID}/";

    public int PricesPerPage { get; set; } = 100;

    public static readonly string MarketableItemSuffix = "marketable";

    public static readonly string PriceBaseUrl = $$"""https://universalis.app/api/v2/""";
    public static readonly string SelectiveColumnsMulti = $$"""
?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ,items.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ%2Citems.lastUploadTime
""";

    public static readonly string SelectiveColumnsSingle = $$"""
?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
