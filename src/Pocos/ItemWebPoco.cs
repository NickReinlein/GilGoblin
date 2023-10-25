using System.Text.Json.Serialization;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Pocos;

public class ItemWebPoco : BaseItemPoco
{
    public ItemWebPoco() : base() { }

    [JsonConstructor]
    public ItemWebPoco(
        int id,
        string name,
        string description,
        int iconId,
        int priceLow,
        int priceMid,
        int stackSize,
        int level,
        int canBeHq
    ) : base(
        id,
        name,
        description,
        iconId,
        priceLow,
        priceMid,
        stackSize,
        level,
        canBeHq > 0)
    {
        _canBeHq = canBeHq;
    }

    public ItemWebPoco(BaseItemPoco poco)
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