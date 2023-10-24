using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace GilGoblin.Web;

public interface IMarketableItemIdsFetcher
{
    Task<List<int>> GetMarketableItemIdsAsync();
}

public class MarketableItemIdsFetcher : IMarketableItemIdsFetcher
{
    private readonly ILogger<MarketableItemIdsFetcher> _logger;
    private HttpClient Client { get; set; }

    public MarketableItemIdsFetcher(ILogger<MarketableItemIdsFetcher> logger, HttpClient? client = null)
    {
        _logger = logger;
        Client = client ?? new HttpClient();
    }

    public MarketableItemIdsFetcher(ILogger<MarketableItemIdsFetcher> logger)
    {
        _logger = logger;
    }

    public async Task<List<int>> GetMarketableItemIdsAsync()
    {
        try
        {
            var fullPath = string.Concat(BasePath, PathSuffix);

            var response = await Client.GetAsync(fullPath);
            if (!response.IsSuccessStatusCode)
                throw new WebException();

            var returnedList = await response.Content.ReadFromJsonAsync<List<int>>();
            return returnedList.Where(i => i > 0).ToList();
        }
        catch
        {
            _logger.LogError("Failure during call to get marketable item Ids");
            return new List<int>();
        }
    }

    private static string PathSuffix => "marketable";
    private static string BasePath => "https://universalis.app/api/v2/";
}