using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class PriceFetcher
    : DataFetcher<PriceWebResponsePoco, PriceWebPoco>,
        IPriceRepository<PriceWebPoco>
{
    public PriceFetcher() : base(_priceBaseUrl) { }

    public PriceFetcher(HttpClient client) : base(_priceBaseUrl, client) { }

    public async Task<PriceWebPoco?> Get(int worldID, int id)
    {
        var worldString = GetWorldString(worldID);
        var idString = $"{id}";
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsSingle });

        return await base.GetAsync(pathSuffix);
    }

    public async Task<IEnumerable<PriceWebPoco?>> GetMultiple(int worldID, IEnumerable<int> ids)
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
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsMulti });

        return await base.GetMultipleAsync(pathSuffix);
    }

    public async Task<IEnumerable<PriceWebPoco>> GetAll(int worldID)
    {
        var allIDs = await GetMarketableItemIDs();
        if (!allIDs.Any())
            return Array.Empty<PriceWebPoco>();

        var results = await GetMultiple(worldID, allIDs);

        var successes = results
            .Cast<PriceWebPoco>()
            .Where(i => i is not null && i.ItemID > 0)
            .ToList();
        return successes;
    }

    public async Task<List<int>> GetMarketableItemIDs()
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
