using System.Collections.Generic;

namespace GilGoblin.Api.Controllers;

public interface IWorldController
{
    Dictionary<int, string> GetAllWorlds();
    KeyValuePair<int, string> GetWorld(int id);
}