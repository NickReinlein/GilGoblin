using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IPriceRepository
{
    Task<PricePoco?> Get(int worldID, int id);
    Task<IEnumerable<PricePoco?>> GetMultiple(int worldID, IEnumerable<int> id);
    Task<IEnumerable<PricePoco>> GetAll(int worldID);
}
