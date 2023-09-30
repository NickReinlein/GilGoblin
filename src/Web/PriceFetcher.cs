using System.Net.Http.Json;
using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace GilGoblin.Web;

public class PriceFetcher : DataFetcher<PriceWebPoco, PriceWebResponse>, IPriceDataFetcher
{
    private readonly ILogger<PriceFetcher> _logger;

    public PriceFetcher(ILogger<PriceFetcher> logger)
        : base(PriceBaseUrl, logger)
    {
        _logger = logger;
    }

    public PriceFetcher(HttpClient client, ILogger<PriceFetcher> logger)
        : base(PriceBaseUrl, logger, client)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<PriceWebPoco?>> FetchPricesAsync(
        int worldId,
        IEnumerable<int> ids
    )
    {
        var idString = string.Empty;
        var worldString = GetWorldString(worldId);
        var idList = ids.ToList();
        if (!idList.Any())
            return Array.Empty<PriceWebPoco>();

        var sb = new StringBuilder();
        foreach (var id in idList)
        {
            sb.Append(id);
            if (id != idList.Last())
                sb.Append(',');
        }

        var pathSuffix = string.Concat(new[] { worldString, idString, SelectiveColumnsMulti });

        var response = await FetchAndSerializeDataAsync(pathSuffix);

        return response is not null ? response.GetContentAsList() : new List<PriceWebPoco>();
    }

    public async Task<List<List<int>>> GetIdsAsBatchJobsAsync()
    {
        var allIDs = await GetMarketableItemIdsAsync();
        if (!allIDs.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(PricesPerPage);
        return batcher.SplitIntoBatchJobs(allIDs);
    }

    public async Task<List<int>> GetMarketableItemIdsAsync()
    {
        try
        {
            var fullPath = string.Concat(BasePath, MarketableItemSuffix);

            var response = await Client.GetAsync(fullPath);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failure response to get marketable item Ids");
                return new List<int>();
            }

            var returnedList = await response.Content.ReadFromJsonAsync<List<int>>();
            return returnedList.Where(i => i > 0).ToList();
        }
        catch
        {
            _logger.LogError("Failure during call to get marketable item Ids");
            return new List<int>();
        }
    }

    public static string GetWorldString(int worldID) => $"{worldID}/";

    public int PricesPerPage { get; set; } = 100;

    public static readonly string MarketableItemSuffix = "marketable";

    public static readonly string PriceBaseUrl = $"https://universalis.app/api/v2/";

    public static readonly string SelectiveColumnsMulti =
        $"?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ,items.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ%2Citems.lastUploadTime";
}