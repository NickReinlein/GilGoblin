using System.Collections.Generic;

namespace GilGoblin.Fetcher.Pocos;

public interface IResponseToList<T> where T : class
{
    List<T> GetContentAsList();
}
