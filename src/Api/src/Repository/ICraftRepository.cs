using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Api.Repository;

public interface ICraftRepository<T>
    where T : class
{
    Task<T> GetBestCraftForItem(int worldId, int itemId);
    Task<List<T>> GetBestCraftsForWorld(int worldId);
}