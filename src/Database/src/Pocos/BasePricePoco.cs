using CsvHelper.Configuration.Attributes;

namespace GilGoblin.Database.Pocos;

public class BasePricePoco : IIdentifiable
{
    [Name("WorldID")] public int WorldId { get; set; }
    [Name("ItemID")] public int ItemId { get; set; }

    // The last upload time for this endpoint, in milliseconds since the UNIX epoch.
    public long LastUploadTime { get; set; }

    public int GetId() => ItemId;
}