using GilGoblin.Pocos;

namespace GilGoblin.Database;

public interface IItemGateway
{
    public ItemInfoPoco GetItem(int itemID);
    public IEnumerable<ItemInfoPoco> GetItems(IEnumerable<int> itemIDs);
    public IEnumerable<ItemInfoPoco> GetAllItems();
}
