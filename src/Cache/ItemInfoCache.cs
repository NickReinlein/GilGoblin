using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface IItemCache : IDataCache<int, ItemInfoPoco> { }

public class ItemCache : DataCache<int, ItemInfoPoco>, IItemCache { }
