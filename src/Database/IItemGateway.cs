using GilGoblin.Pocos;

namespace GilGoblin.Database;

public interface IItemGateway
{
    public Task<ItemInfoPoco> GetItem(int itemID);
    public Task<IEnumerable<ItemInfoPoco>> GetItems(IEnumerable<int> itemIDs);
    public Task<IEnumerable<ItemInfoPoco>> GetAllItems();
}
