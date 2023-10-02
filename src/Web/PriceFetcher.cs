using System.Net.Http.Json;
using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;

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

    protected override string GetUrlPathFromEntries(IEnumerable<int> ids, int? worldId = null)
    {
        var basePath = base.GetUrlPathFromEntries(ids, worldId);
        return string.Concat(new[] { basePath, SelectiveColumnsMulti });
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

    public int PricesPerPage { get; set; } = 100;

    private static string MarketableItemSuffix => "marketable";
    private static string PriceBaseUrl => "https://universalis.app/api/v2/";

    private static string SelectiveColumnsMulti =>
        "?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ,items.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ%2Citems.lastUploadTime";
}