using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IItemGateway
{
    public ItemInfoPoco GetItem(int itemID);
    public IEnumerable<ItemInfoPoco> GetItems(IEnumerable<int> itemIDs);
    public IEnumerable<ItemInfoPoco> GetAllItems();
}