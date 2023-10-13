using GilGoblin.Database.Pocos;

namespace GilGoblin.Cache;

public interface IItemInfoCache : IDataCache<int, ItemInfoPoco> { }

public class ItemInfoCache : DataCache<int, ItemInfoPoco>, IItemInfoCache { }
