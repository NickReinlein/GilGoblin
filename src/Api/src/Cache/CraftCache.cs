using GilGoblin.Api.Crafting;

namespace GilGoblin.Api.Cache;

public interface ICraftCache : IDataCache<(int, int), CraftSummaryPoco> { }

public class CraftCache : DataCache<(int, int), CraftSummaryPoco>, ICraftCache { }