using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GilGoblin.Fetcher;

public interface IMarketableItemIdsFetcher
{
    Task<List<int>> GetMarketableItemIdsAsync();
}

public class MarketableItemIdsFetcher(ILogger<MarketableItemIdsFetcher> logger, HttpClient? client = null)
    : IMarketableItemIdsFetcher
{
    private HttpClient Client { get; set; } = client ?? new HttpClient();

    public async Task<List<int>> GetMarketableItemIdsAsync()
    {
        try
        {
            var fullPath = string.Concat(BasePath, PathSuffix);

            var response = await Client.GetAsync(fullPath);
            if (!response.IsSuccessStatusCode)
                throw new WebException();

            var returnedList = await response.Content.ReadFromJsonAsync<List<int>>();
            return returnedList?.Where(i => i > 0).ToList() ?? [];
        }
        catch
        {
            logger.LogError("Failure during call to get marketable item Ids");
            return new List<int>();
        }
    }

    private static string PathSuffix => "marketable";
    private static string BasePath => "https://universalis.app/api/v2/";
}