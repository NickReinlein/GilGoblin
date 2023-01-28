namespace GilGoblin.Repository;

public interface IDataRepository<T> where T : class
{
    Task<T?> Get(int id);
    Task<IEnumerable<T?>> GetAll();
}
