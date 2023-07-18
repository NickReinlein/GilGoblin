using System.Threading.Tasks;

namespace GilGoblin.Web;

public interface IDataFetcher<T, U>
    where T : class
    where U : class
{
    Task<T?> GetAsync(string path);
    Task<U?> GetMultipleAsync(string path);
}
