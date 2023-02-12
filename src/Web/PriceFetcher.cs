using System.Text.Json;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class PriceFetcher : DataFetcher<PriceWebPoco>, IPriceRepository<PriceWebPoco>
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
        throw new NotImplementedException();
    }

    public string GetWorldString(int worldID) => $"{worldID}/";

    private static readonly string _priceBaseUrl = $$"""https://universalis.app/api/v2/""";
    private static readonly string _selectiveColumnsMulti = $$"""
?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";

    private static readonly string _selectiveColumnsSingle = $$"""
?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
