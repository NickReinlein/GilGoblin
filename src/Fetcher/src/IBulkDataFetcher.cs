using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

// ReSharper disable once UnusedTypeParameter
// 'U' is used to implement the IResponseToList<T> interface/restriction
public interface IBulkDataFetcher<T, U> : IDataFetcher<T>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    int GetEntriesPerPage();
    void SetEntriesPerPage(int count);
}