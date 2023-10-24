using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Web;

public interface IBulkDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null);
}