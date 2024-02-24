using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IWorldCache : IDataCache<int, WorldPoco>
{
}

public class WorldCache : DataCache<int, WorldPoco>, IWorldCache
{
}