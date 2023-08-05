using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Repository;

public interface IPriceRepository<T>
    where T : class
{
    T? Get(int worldID, int id);
    IEnumerable<T> GetMultiple(int worldID, IEnumerable<int> ids);
    IEnumerable<T> GetAll(int worldID);
}
