using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

public interface IPriceAggregatedFetcher : IBulkDataFetcher<PriceWebPoco, PriceWebResponse>
{
}

public class PriceAggregatedFetcher : BulkDataFetcher<PriceAggregatedWebPoco, PriceAggregatedWebResponse>, IPriceFetcher
{
    public PriceAggregatedFetcher(
        ILogger<PriceAggregatedFetcher> logger,
        HttpClient? client = null)
        : base(PriceBaseUrl, logger, client)
    {
    }

    private static string PriceBaseUrl => "https://universalis.app/api/v2/aggregated/";
    public Task<List<PriceWebPoco>> FetchByIdsAsync(CancellationToken ct, IEnumerable<int> ids, int? world = null)
    {
        throw new System.NotImplementedException();
    }
}