using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Repository;

public interface IPriceRepository<T> where T : class
{
    Task<T?> Get(int worldID, int id);
    Task<IEnumerable<T?>> GetMultiple(int worldID, IEnumerable<int> ids);
    Task<IEnumerable<T>> GetAll(int worldID);
}
