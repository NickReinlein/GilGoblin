using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IItemInfoFetcher : IDataFetcher<ItemInfoWebPoco, ItemInfoWebResponse>
{
    Task<List<int>> GetMarketableItemIDsAsync();
    Task<List<List<int>>> GetAllIDsAsBatchJobsAsync();
    Task<ItemInfoWebPoco> FetchItemInfoAsync(int id);
    Task<List<ItemInfoWebPoco?>> FetchMultipleItemsAsync(IEnumerable<int> ids);
}
