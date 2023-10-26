using GilGoblin.Pocos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;

namespace GilGoblin.Fetcher;

public interface IPriceFetcher : IBulkDataFetcher<PriceWebPoco, PriceWebResponse>
{
}

public class PriceFetcher : BulkDataFetcher<PriceWebPoco, PriceWebResponse>, IPriceFetcher
{
    public PriceFetcher(
        IMarketableItemIdsFetcher marketableFetcher,
        ILogger<PriceFetcher> logger,
        HttpClient? client = null)
        : base(PriceBaseUrl, marketableFetcher, logger, client)
    {
    }

    protected override string GetUrlPathFromEntries(IEnumerable<int> ids, int? worldId = null)
    {
        var basePath = base.GetUrlPathFromEntries(ids, worldId);
        return string.Concat(new[] { basePath, SelectiveColumnsMulti });
    }

    private static string PriceBaseUrl => "https://universalis.app/api/v2/";

    private static string SelectiveColumnsMulti =>
        "?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ,items.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ%2Citems.lastUploadTime";
}