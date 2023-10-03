using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.DataUpdater;

namespace GilGoblin.Web;

public interface ISingleDataFetcher<T> where T : class, IIdentifiable
{
    Task<T?> FetchByIdAsync(int id, int? world = null);
}