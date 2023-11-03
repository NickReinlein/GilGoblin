using System.Collections.Generic;

namespace GilGoblin.Fetcher;

public interface IResponseToList<T> where T : class
{
    List<T> GetContentAsList();
}
