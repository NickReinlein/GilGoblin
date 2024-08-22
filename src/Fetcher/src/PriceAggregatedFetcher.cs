using Microsoft.Extensions.Logging;
using System.Net.Http;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

public interface IPriceAggregatedFetcher : IBulkDataFetcher<PriceAggregatedWebPoco, PriceAggregatedWebResponse>;

public class PriceAggregatedFetcher(
    ILogger<PriceAggregatedFetcher> logger,
    HttpClient? client = null)
    : BulkDataFetcher<PriceAggregatedWebPoco, PriceAggregatedWebResponse>(PriceBaseUrl, logger, client),
        IPriceAggregatedFetcher
{
    private static string PriceBaseUrl => "https://universalis.app/api/v2/aggregated/";
}