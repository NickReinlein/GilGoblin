using System.Text.Json;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public class PriceFetcher : DataFetcher<PricePoco>, IPriceRepository
{
    // private static readonly string _fullPath =
    //     $$"""https://universalis.app/api/v2/34/5050,5060?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ""";
    public static readonly string PriceBaseUrl = $$"""https://universalis.app/api/v2/""";
    public static readonly string SelectiveColumns =
        $$"""?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ%2Citems.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ""";

    public PriceFetcher() : base(PriceBaseUrl) { }

    public string GetWorldString(int worldID) => $"{worldID}/";

    public async Task<PricePoco?> Get(int worldID, int id)
    {
        var idString = $"{id}";
        var worldString = GetWorldString(worldID);
        var entirePath = string.Concat(
            new[] { PriceBaseUrl, worldString, idString, SelectiveColumns }
        );
        return await base.GetAsync(entirePath);
    }

    public async Task<IEnumerable<PricePoco?>> GetMultiple(int worldID, IEnumerable<int> id)
    {
        var idString = $"{id}";
        var worldString = GetWorldString(worldID);
        var entirePath = string.Concat(
            new[] { PriceBaseUrl, worldString, idString, SelectiveColumns }
        );
        return await base.GetMultipleAsync(entirePath);
    }

    public async Task<IEnumerable<PricePoco>> GetAll(int worldID)
    {
        throw new NotImplementedException();
    }
}
