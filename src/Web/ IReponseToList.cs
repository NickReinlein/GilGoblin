namespace GilGoblin.Web;

public interface IReponseToList<T> where T : class
{
    List<T> GetContentAsList();
}
