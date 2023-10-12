using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace GilGoblin.Database.Pocos;

public class BaseItemInfoPoco : IIdentifiable
{
    [Name("#")] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    [Name("Icon")] public int? IconId { get; set; }

    [Name("Level{Item}")] public int? Level { get; set; }
    public int? StackSize { get; set; }
    [Name("Price{Mid}")] public int? PriceMid { get; set; }
    [Name("Price{Low}")] public int? PriceLow { get; set; }
    public bool CanBeHq { get; set; }

    [JsonConstructor]
    public BaseItemInfoPoco() { }

    public BaseItemInfoPoco(
        int id,
        string name,
        string description,
        int? iconId,
        int? level,
        int? priceLow,
        int? priceMid,
        int? stackSize,
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