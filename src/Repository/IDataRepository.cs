using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Repository;

public interface IDataRepository<T> where T : class
{
    Task<T?> Get(int id);
    Task<IEnumerable<T?>> GetMultiple(IEnumerable<int> ids);
    Task<IEnumerable<T>> GetAll();
}
