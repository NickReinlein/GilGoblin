using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.DataUpdater;

namespace GilGoblin.Web;

public interface IDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null);
}