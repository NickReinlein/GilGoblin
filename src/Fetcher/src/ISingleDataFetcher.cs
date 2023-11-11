using System.Net.Http;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher;

public interface ISingleDataFetcher<T> : IDataFetcher<T>
    where T : class, IIdentifiable
{
    Task<T?> FetchByIdAsync(int id, int? world = null);
    Task<T> ReadResponseContentAsync(HttpContent content);
}