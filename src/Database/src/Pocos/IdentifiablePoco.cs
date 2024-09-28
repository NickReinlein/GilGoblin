namespace GilGoblin.Database.Pocos;

public abstract record IdentifiablePoco : IIdentifiable
{
    public int Id { get; init; }
    public int GetId() => Id;
}