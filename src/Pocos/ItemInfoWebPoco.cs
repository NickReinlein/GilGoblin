using GilGoblin.Database.Pocos;

namespace GilGoblin.Pocos;

public class ItemInfoWebPoco : BaseItemInfoPoco
{
    public ItemInfoWebPoco() : base() { }

    public ItemInfoWebPoco(
        int id,
        string name,
        string description,
        int iconId,
        int priceLow,
        int priceMid,
        int stackSize,
        int level,
        bool canBeHq
    ) : base(
        id,
        name,
        description,
        iconId,
        priceLow,
        priceMid,
        stackSize,
        level,
        canBeHq)
    {
    }

    public ItemInfoWebPoco(BaseItemInfoPoco poco)
    {
        Id = poco.Id;
        Description = poco.Description;
        Name = poco.Name;
        Level = poco.Level;
        StackSize = poco.StackSize;
        PriceLow = poco.PriceLow;
        PriceMid = poco.PriceMid;
        CanBeHq = poco.CanBeHq;
        IconId = poco.IconId;
    }
}