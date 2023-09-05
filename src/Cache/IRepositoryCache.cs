using System.Threading.Tasks;

namespace GilGoblin.Cache
{
    public interface IRepositoryCache
    {
        Task FillCache();
    }
}
