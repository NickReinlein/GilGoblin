using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace GilGoblin.Pocos;

public class ItemInfoPoco
{
    [Name("#")]
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    [Name("Icon")]
    public int IconID { get; set; }

    [Name("Level{Item}")]
    public int Level { get; set; }

    [Name("Price{Mid}")]
    public int VendorPrice { get; set; }
    public int StackSize { get; set; }
    public bool CanBeHq { get; set; }

    [JsonConstructor]
    public ItemInfoPoco() { }

    public ItemInfoPoco(
        int id,
        string name,
        string description,
        int iconID,
        int priceMid,
        int stackSize,
        int level,
        bool canBeHq
    )
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
