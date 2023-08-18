using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface ICostCache : IDataCache<(int, int), CostPoco> { }

public class CostCache : DataCache<(int, int), CostPoco>, ICostCache { }
