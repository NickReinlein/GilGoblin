using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IPriceDataFetcher : IDataFetcher<PriceWebPoco, PriceWebResponse>
{
    Task<List<int>> GetMarketableItemIDsAsync();
    Task<List<List<int>>> GetAllIDsAsBatchJobsAsync(int worldID);
    Task<PriceWebPoco?> FetchPriceAsync(int worldID, int id);
    Task<IEnumerable<PriceWebPoco?>> FetchMultiplePricesAsync(int worldID, IEnumerable<int> ids);
}
