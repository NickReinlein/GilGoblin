using GilGoblin.Api.Pocos;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface ICraftCache : IDataCache<TripleKey, CraftSummaryPoco>;

public class CraftCache : DataCache<TripleKey, CraftSummaryPoco>, ICraftCache;