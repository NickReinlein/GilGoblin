namespace GilGoblin.Database.Pocos;

public record IdentifiableTripleKeyPoco(
    int ItemId,
    int WorldId,
    bool IsHq)
    : IdentifiablePoco
{
    public (int, int, bool) GetKey() => (ItemId, WorldId, IsHq);
}