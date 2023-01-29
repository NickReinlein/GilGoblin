namespace GilGoblin.Repository;

public interface IDataRepository<T> where T : class
{
    T? Get(int id);
    IEnumerable<T?> GetAll();
}
