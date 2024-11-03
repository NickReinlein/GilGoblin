namespace GilGoblin.Database.Pocos;

public record TripleKey(int ItemId, int WorldId, bool IsHq)
{
    public (int, int, bool) GetKey() => (ItemId, WorldId, IsHq);
}