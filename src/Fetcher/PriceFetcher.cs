using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;

namespace GilGoblin.Fetcher;

public interface IPriceFetcher : IBulkDataFetcher<PriceWebPoco, PriceWebResponse>
{
    Task<List<List<int>>> GetIdsAsBatchJobsAsync();
}

public class PriceFetcher : BulkDataFetcher<PriceWebPoco, PriceWebResponse>, IPriceFetcher
{
    private readonly IMarketableItemIdsFetcher _marketableFetcher;

    public PriceFetcher(
        IMarketableItemIdsFetcher marketableFetcher,
        ILogger<PriceFetcher> logger,
        HttpClient? client = null)
        : base(PriceBaseUrl, logger, client)
    {
        _marketableFetcher = marketableFetcher;
    }

    protected override string GetUrlPathFromEntries(IEnumerable<int> ids, int? worldId = null)
    {
        var basePath = base.GetUrlPathFromEntries(ids, worldId);
        return string.Concat(new[] { basePath, SelectiveColumnsMulti });
    }

    public async Task<List<List<int>>> GetIdsAsBatchJobsAsync()
    {
        var allIds = await _marketableFetcher.GetMarketableItemIdsAsync();
        if (!allIds.Any())
            return new List<List<int>>();

        var batcher = new Batcher<int>(GetEntriesPerPage());
        return batcher.SplitIntoBatchJobs(allIds);
    }

    private static string PriceBaseUrl => "https://universalis.app/api/v2/";

    private static string SelectiveColumnsMulti =>
        "?listings=0&entries=0&fields=items.itemID%2Citems.worldID%2Citems.currentAveragePrice%2Citems.currentAveragePriceNQ%2Citems.currentAveragePriceHQ,items.averagePrice%2Citems.averagePriceNQ%2Citems.averagePriceHQ%2Citems.lastUploadTime";
}