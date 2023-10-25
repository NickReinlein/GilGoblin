using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher;

public interface IBulkDataFetcher<T, U> : IDataFetcher<T>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
}