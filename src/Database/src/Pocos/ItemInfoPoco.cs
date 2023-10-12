namespace GilGoblin.Database.Pocos;

public class ItemInfoPoco : BaseItemInfoPoco
{
    public ItemInfoPoco() { }

    public ItemInfoPoco(
        int id,
        string name,
        string description,
        int? iconId,
        int? level,
        int? priceLow,
        int? priceMid,
        int? stackSize,
        bool canBeHq
    ) : base(
        id,
        name,
        description,
        iconId,
        level,
        priceLow,
        priceMid,
        stackSize,
        canBeHq)
    {
        Id = id;
        IconId = iconId;
        Description = description;
        Name = name;
        PriceMid = priceMid;
        StackSize = stackSize;
        Level = level;
        CanBeHq = canBeHq;
    }
}