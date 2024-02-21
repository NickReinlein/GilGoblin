using System.Collections.Generic;
using System.Threading.Tasks;

namespace GilGoblin.Api.Repository;

public interface IWorldRepository
{
    Dictionary<int, string> GetAllWorlds();
    KeyValuePair<int, string> GetWorld(int id);
}