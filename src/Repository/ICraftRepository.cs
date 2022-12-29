namespace GilGoblin.Repository;

public interface ICraftRepository<T> where T : class
{
    public T GetCraft(int worldId, int id);
    public IEnumerable<T> GetBestCrafts(int worldId);
}
