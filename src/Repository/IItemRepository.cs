using GilGoblin.Pocos;
using GilGoblin.Cache;

namespace GilGoblin.Repository;

public interface IItemRepository : IDataRepository<ItemInfoPoco>, IRepositoryCache { }
