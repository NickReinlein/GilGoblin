using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class PriceGateway : IPriceRepository<PricePoco>
{
    private readonly IPriceRepository<PricePoco> _prices;

    public PriceGateway(IPriceRepository<PricePoco> prices)
    {
        _prices = prices;
    }

    public async Task<PricePoco?> Get(int worldID, int id) => await _prices.Get(worldID, id);

    public async Task<IEnumerable<PricePoco?>> GetMultiple(int worldID, IEnumerable<int> ids) =>
        await _prices.GetMultiple(worldID, ids);

    public async Task<IEnumerable<PricePoco>> GetAll(int worldID) => await _prices.GetAll(worldID);
}
