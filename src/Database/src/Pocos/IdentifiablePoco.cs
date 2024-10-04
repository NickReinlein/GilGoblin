namespace GilGoblin.Database.Pocos;

public interface IIdentifiable
{
    int GetId();
}

public abstract record IdentifiablePoco : IIdentifiable
{
    public int Id { get; init; }
    public int GetId() => Id;
}