using System.Threading.Tasks;

namespace GilGoblin.Cache
{
    public interface IRepositoryCache
    {
        public Task FillCache();
    }
}
