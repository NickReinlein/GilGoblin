using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Fetcher;

public interface IItemSingleFetcher : ISingleDataFetcher<ItemWebPoco>
{
    public Task<List<List<int>>> GetIdsAsBatchJobsAsync();
}