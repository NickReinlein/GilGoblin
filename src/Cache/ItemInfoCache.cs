using GilGoblin.Database.Pocos;

namespace GilGoblin.Cache;

public interface IItemCache : IDataCache<int, ItemPoco> { }

public class ItemCache : DataCache<int, ItemPoco>, IItemCache { }
