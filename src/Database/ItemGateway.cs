using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class ItemGateway : IItemRepository
{
    private readonly IItemRepository _items;

    public ItemGateway(IItemRepository items)
    {
        _items = items;
    }

    public async Task<ItemInfoPoco?> Get(int itemID) => await _items.Get(itemID);

    public async Task<IEnumerable<ItemInfoPoco?>> GetMultiple(IEnumerable<int> itemIDs) =>
        await _items.GetMultiple(itemIDs);

    public async Task<IEnumerable<ItemInfoPoco>> GetAll() => await _items.GetAll();
}
