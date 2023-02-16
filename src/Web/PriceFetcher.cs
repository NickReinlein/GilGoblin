using System.Diagnostics;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Services;

namespace GilGoblin.Web;

public class PriceFetcher : DataFetcher<PriceWebPoco, PriceWebResponsePoco>, IPriceDataFetcher
{
    private readonly ILogger<PriceFetcher> _logger;

    public PriceFetcher(ILogger<PriceFetcher> logger) : base(_priceBaseUrl, null, logger)
    {
        _logger = logger;
    }

    public PriceFetcher(HttpClient client, ILogger<PriceFetcher> logger)
        : base(_priceBaseUrl, client, logger)
    {
        _logger = logger;
    }

    public async Task<PriceWebPoco?> FetchPriceAsync(int worldID, int id)
    {
        _logger.LogInformation("Fetching for world {World}, 1 item: {ID}", worldID, id);
        var worldString = GetWorldString(worldID);
        var idString = $"{id}";
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsSingle });

        return await base.GetAsync(pathSuffix);
    }

    public async Task<IEnumerable<PriceWebPoco?>> FetchMultiplePricesAsync(
        int worldID,
        IEnumerable<int> ids
    )
    {
        _logger.LogInformation(
            "Fetching for world {World}, {Count} items, starting with id: ",
            worldID,
            ids.Count(),
            ids.FirstOrDefault()
        );
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
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsMulti });

        var response = await base.GetMultipleAsync(pathSuffix);

        return response is not null ? response.GetContentAsList() : new List<PriceWebPoco>();
    }

    public async Task<List<List<int>>> GetAllIDsAsBatchJobsAsync(int worldID)
    {
        _logger.LogWarning("Fetching for world {World}, all items", worldID);
        var allIDs = await GetMarketableItemIDsAsync();
        if (!allIDs.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(PricesPerPage);
        return batcher.SplitIntoBatchJobs(allIDs);
    }

    public async Task<List<int>> GetMarketableItemIDsAsync()
    {
        var fullPath = string.Concat(_basePath, _marketableItemSuffix);
        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return new List<int>();

        var results = await response.Content.ReadFromJsonAsync<List<int>>();
        if (results is null)
            return new List<int>();

        return results?.Cast<int>().Where(i => i > 0).ToList() ?? new List<int>();
    }

    public string GetWorldString(int worldID) => $"{worldID}/";

    public int PricesPerPage { get; set; } = 100;

    private static readonly string _marketableItemSuffix = "marketable";

    private static readonly string _priceBaseUrl = $$"""https://universalis.app/api/v2/""";
    private static readonly string _selectiveColumnsMulti = $$"""
?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";

    private static readonly string _selectiveColumnsSingle = $$"""
?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
