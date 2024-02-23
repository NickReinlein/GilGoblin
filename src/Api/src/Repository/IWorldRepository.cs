using System.Collections.Generic;

namespace GilGoblin.Api.Repository;

public interface IWorldRepository
{
    Dictionary<int, string> GetAllWorlds();
    KeyValuePair<int, string> GetWorld(int id);
}