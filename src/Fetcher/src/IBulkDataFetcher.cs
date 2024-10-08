using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

public interface IBulkDataFetcher<T, U> : IDataFetcher<T>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    int GetEntriesPerPage();
    void SetEntriesPerPage(int count);
}