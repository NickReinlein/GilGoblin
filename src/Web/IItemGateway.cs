using GilGoblin.Pocos;

namespace GilGoblin.Web
{
    public interface IItemGateway
    {
        public ItemInfoPoco GetItem(int itemID);
        public ItemInfoPoco GetItems(IEnumerable<int> itemIDs);
    }
}