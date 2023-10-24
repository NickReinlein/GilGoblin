using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher;

public interface IBulkDataFetcher<T> : IDataFetcher<T> where T : class, IIdentifiable
{ }