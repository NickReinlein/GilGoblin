using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IPriceRepository
{
    PricePoco Get(int worldID, int id);
    IEnumerable<PricePoco> GetAll(int worldID);
}
