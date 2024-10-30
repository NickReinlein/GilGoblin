namespace GilGoblin.Database.Pocos;

public record IdentifiableTripleKeyPoco(
    int ItemId,
    int WorldId,
    bool IsHq)
    : TripleKey(ItemId, WorldId, IsHq), IIdentifiable
{
    public int Id { get; init; }
    public int GetId() => Id;
}