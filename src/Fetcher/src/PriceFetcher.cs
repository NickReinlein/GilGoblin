using Microsoft.Extensions.Logging;
using System.Net.Http;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

public interface IPriceFetcher : IBulkDataFetcher<PricePoco, PriceWebResponse>;

public class PriceFetcher(
    ILogger<PriceFetcher> logger,
    HttpClient? client = null)
    : BulkDataFetcher<PricePoco, PriceWebResponse>(PriceBaseUrl, logger, client),
        IPriceFetcher
{
    private static string PriceBaseUrl => "https://universalis.app/api/v2/aggregated/";
}