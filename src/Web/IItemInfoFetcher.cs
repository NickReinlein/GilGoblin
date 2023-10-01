using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IItemInfoFetcher : IDataFetcher<ItemInfoWebPoco>
{
    Task<List<int>> GetMarketableItemIDsAsync();
    Task<List<List<int>>> GetIdsAsBatchJobsAsync();
}