using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using GilGoblin.DataUpdater;

namespace GilGoblin.Pocos;

public class BaseItemInfoPoco : IIdentifiable
{
    [Name("#")] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    [Name("Icon")] public int IconId { get; set; }

    [Name("Level{Item}")] public int Level { get; set; }
    [Name("Price{Low}")] public int PriceLow { get; set; }
    [Name("Price{Mid}")] public int PriceMid { get; set; }
    public int StackSize { get; set; }
    public bool CanBeHq { get; set; }

    [JsonConstructor]
    public BaseItemInfoPoco() { }

    public BaseItemInfoPoco(
        int id,
        string name = "",
        string description = "",
        int iconId = 0,
        int level = 0,
        int priceLow = 0,
        int priceMid = 0,
        int stackSize = 0,
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