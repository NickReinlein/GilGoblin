using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IItemCache : IDataCache<int, ItemPoco> { }

public class ItemCache : DataCache<int, ItemPoco>, IItemCache { }
