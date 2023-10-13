using GilGoblin.Pocos;

namespace GilGoblin.Cache;

public interface ICraftCache : IDataCache<(int, int), CraftSummaryPoco> { }

public class CraftCache : DataCache<(int, int), CraftSummaryPoco>, ICraftCache { }
