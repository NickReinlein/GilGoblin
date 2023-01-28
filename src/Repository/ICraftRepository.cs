namespace GilGoblin.Repository;

public interface ICraftRepository<T> where T : class
{
    Task<T?> GetCraft(int worldId, int id);
    Task<IEnumerable<T?>> GetBestCrafts(int worldId);
}
