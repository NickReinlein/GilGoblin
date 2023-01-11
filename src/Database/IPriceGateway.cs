using GilGoblin.Pocos;

namespace GilGoblin.Database;

public interface IPriceGateway
{
    public PricePoco GetPrice(int worldID, int itemID);
    public IEnumerable<PricePoco> GetPrices(int worldID, IEnumerable<int> itemIDs);
    public IEnumerable<PricePoco> GetAllPrices(int worldID);
}
