using System.Threading.Tasks;

namespace GilGoblin.Api.Cache
{
    public interface IRepositoryCache
    {
        Task FillCache();
    }
}
