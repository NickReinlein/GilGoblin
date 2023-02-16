using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IPriceRepository<T> where T : class
{
    Task<T?> Get(int worldID, int id);
    Task<IEnumerable<T?>> GetMultiple(int worldID, IEnumerable<int> id);
    Task<IEnumerable<T>> GetAll(int worldID);
}
