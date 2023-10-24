using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher;

public interface IDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null);
}