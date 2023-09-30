using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using GilGoblin.DataUpdater;

namespace GilGoblin.Pocos;

public class BaseItemInfoPoco: IIdentifiable
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
    public BaseItemInfoPoco() { }

    public BaseItemInfoPoco(
        int id,
        string name = "",
        string description = "",
        int iconID = 0,
        int level = 0,
        int vendorPrice = 0,
        int stackSize = 0,
        bool canBeHq = false)
    {
        ID = id;
        Name = name;
        Description = description;
        IconID = iconID;
        Level = level;
        VendorPrice = vendorPrice;
        StackSize = stackSize;
        CanBeHq = canBeHq;
    }
    public int GetId() => ID;
}