using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Repository;

public interface ICraftRepository<T>
    where T : class
{
    Task<T?> GetCraft(int worldId, int id);
    IEnumerable<T> GetBestCrafts(int worldId);
}
