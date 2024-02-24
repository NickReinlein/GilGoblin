using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Controllers;

public interface IWorldController
{
    IEnumerable<WorldPoco> GetAllWorlds();
    WorldPoco GetWorld(int id);
}