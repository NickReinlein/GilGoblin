using System.Text.Json.Serialization;

namespace GilGoblin.Database.Pocos;

public class BaseItemPoco : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public int IconId { get; set; }
    public int Level { get; set; }
    public int StackSize { get; set; }
    public int PriceMid { get; set; }
    public int PriceLow { get; set; }
    public bool CanBeHq { get; set; }

    [JsonConstructor]
    public BaseItemPoco() { }

    public BaseItemPoco(
        int id,
        string name,
        string description,
        int iconId,
        int level,
        int priceMid,
        int priceLow,
        int stackSize,
        bool canBeHq = false)
    {
        Id = id;
        Name = name;
        Description = description;
        IconId = iconId;
        Level = level;
        PriceMid = priceMid;
        PriceLow = priceLow;
        StackSize = stackSize;
        CanBeHq = canBeHq;
    }

    public int GetId() => Id;
}