using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Web;

public interface IPriceDataFetcher : IDataFetcher<PriceWebPoco, PriceWebResponsePoco>
{
    Task<List<int>> GetMarketableItemIDs();
    Task<List<List<int>>> GetAllIDsAsBatchJobs(int worldID);
}
