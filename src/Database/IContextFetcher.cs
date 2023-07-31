using System.Threading.Tasks;

namespace GilGoblin.Database;

public interface IContextFetcher
{
    public Task<GilGoblinDbContext?> GetContextAsync();
}
