using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public class WorldWebPoco(int id, string? name) : IIdentifiable
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name ?? string.Empty;

    public int GetId() => Id;
}