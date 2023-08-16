using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Repository;

public interface ICraftRepository<T>
    where T : class
{
    Task<T?> GetBestCraft(int worldId, int id);
    Task<IEnumerable<T>> GetBestCrafts(int worldId);
}
