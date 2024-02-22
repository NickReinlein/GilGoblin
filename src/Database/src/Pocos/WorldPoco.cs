namespace GilGoblin.Database.Pocos;

public class WorldPoco : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int GetId() => Id;
}