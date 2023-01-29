namespace GilGoblin.Repository;

public interface ICraftRepository<T> where T : class
{
    T? GetCraft(int worldId, int id);
    IEnumerable<T?> GetBestCrafts(int worldId);
}
