using System.Text.Json;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class PriceFetcher : DataFetcher<PricePoco>, IPriceRepository
{
    public PriceFetcher() : base(_priceBaseUrl) { }

    public PriceFetcher(HttpClient client) : base(_priceBaseUrl, client) { }

    public string GetWorldString(int worldID) => $"{worldID}/";

    public async Task<PricePoco?> Get(int worldID, int id)
    {
        var idString = $"{id}";
        var worldString = GetWorldString(worldID);
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsSingle });
        return await base.GetAsync(pathSuffix);
    }

    public async Task<IEnumerable<PricePoco?>> GetMultiple(int worldID, IEnumerable<int> id)
    {
        var idString = $"{id}";
        var worldString = GetWorldString(worldID);
        var pathSuffix = string.Concat(new[] { worldString, idString, _selectiveColumnsMulti });
        return await base.GetMultipleAsync(pathSuffix);
    }

    public async Task<IEnumerable<PricePoco>> GetAll(int worldID)
    {
        throw new NotImplementedException();
    }

    private static readonly string _priceBaseUrl = $$"""https://universalis.app/api/v2/""";
    private static readonly string _selectiveColumnsMulti = $$"""
?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ
""";

    private static readonly string _selectiveColumnsSingle = $$"""
?listings=0&entries=0&fields=itemID,worldID,currentAveragePrice,currentAveragePriceNQ,currentAveragePriceHQ,averagePrice,averagePriceNQ,averagePriceHQ,lastUploadTime
""";
}
