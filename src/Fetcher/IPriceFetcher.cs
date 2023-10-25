using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Fetcher;

public interface IPriceBulkDataFetcher : IBulkDataFetcher<PriceWebPoco, PriceWebResponse>
{
    Task<List<List<int>>> GetIdsAsBatchJobsAsync();
    int GetEntriesPerPage();
    void SetEntriesPerPage(int count);
}