namespace GilGoblin.Database.Pocos;

public record TripleKey(int ItemId, int WorldId, bool IsHq) : ITripleKey
{
    public (int, int, bool) GetKey() => (ItemId, WorldId, IsHq);
}

public interface ITripleKey
{
    public (int, int, bool) GetKey();
}