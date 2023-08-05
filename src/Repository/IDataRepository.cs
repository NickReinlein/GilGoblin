using System.Collections.Generic;

namespace GilGoblin.Repository;

public interface IDataRepository<T>
    where T : class
{
    T? Get(int id);
    IEnumerable<T> GetMultiple(IEnumerable<int> ids);
    IEnumerable<T> GetAll();
}
