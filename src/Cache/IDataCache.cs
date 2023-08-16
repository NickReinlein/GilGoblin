using GilGoblin.Repository;

namespace GilGoblin.Cache;

public interface IDataCache<T> : IDataRepository<T>
    where T : class { }
