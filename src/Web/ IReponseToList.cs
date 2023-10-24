using System.Collections.Generic;

namespace GilGoblin.Web;

public interface IResponseToList<T> where T : class
{
    List<T> GetContentAsList();
}
