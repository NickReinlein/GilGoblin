namespace GilGoblin.Database.Pocos;

public record IdentifiableTripleKeyPoco(
    int ItemId,
    int WorldId,
    bool IsHq)
    : TripleKey(ItemId, WorldId, IsHq), IIdentifiable
{
    public int Id { get; set; }
    public int GetId() => Id;
}