using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class PriceGateway : IPriceRepository<PricePoco>
{
    private readonly GilGoblinDatabase _database;
    private readonly ILogger<PriceGateway> _logger;

    public PriceGateway(GilGoblinDatabase database, ILogger<PriceGateway> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<PricePoco?> Get(int worldID, int itemID)
    {
        _logger.LogInformation("Getting price for item {ID} for world {WorldID}", itemID, worldID);
        using var context = await GetContext();
        var price = context?.Price?.FirstOrDefault(x => x.ItemID == itemID && x.WorldID == worldID);
        return price;
    }

    public async Task<IEnumerable<PricePoco?>> GetMultiple(int worlID, IEnumerable<int> itemIDs)
    {
        _logger.LogInformation(
            "Getting {Number} prices for world {WorldID}",
            itemIDs.Count(),
            worlID
        );
        using var context = await GetContext();
        return context
                ?.Price?.Where(i => i.WorldID == worlID && itemIDs.Contains(i.ItemID))
                .ToList() ?? new List<PricePoco>();
    }

    public async Task<IEnumerable<PricePoco>> GetAll(int worldID)
    {
        _logger.LogInformation("Getting all prices for world {WorldID}", worldID);
        using var context = await GetContext();
        return context?.Price?.Where(p => p.WorldID == worldID).ToList() ?? new List<PricePoco>();
    }

    private async Task<GilGoblinDbContext?> GetContext() => await _database.GetContextAsync();
}
