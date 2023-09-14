namespace GilGoblin.Pocos;

public class ItemInfoWebPoco : BaseItemInfoPoco
{
    public ItemInfoWebPoco() : base() { }
    public ItemInfoWebPoco(
        int id,
        string name,
        string description,
        int iconID,
        int priceMid,
        int stackSize,
        int level,
        bool canBeHq
    ) : base(id, name, description, iconID, priceMid, stackSize, level, canBeHq)
    {
        ID = id;
        IconID = iconID;
        Description = description;
        Name = name;
        VendorPrice = priceMid;
        StackSize = stackSize;
        Level = level;
        CanBeHq = canBeHq;
    }
}
