using GilGoblin.DataUpdater;

namespace GilGoblin.Pocos;

public class ItemInfoWebPoco : BaseItemInfoPoco, IIdentifiable
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

    public ItemInfoWebPoco(BaseItemInfoPoco poco)
    {
        ID = poco.ID;
        Description = poco.Description;
        Name = poco.Name;
        Level = poco.Level;
        StackSize = poco.StackSize;
        VendorPrice = poco.VendorPrice;
        CanBeHq = poco.CanBeHq;
        IconID = poco.IconID;
    }
}