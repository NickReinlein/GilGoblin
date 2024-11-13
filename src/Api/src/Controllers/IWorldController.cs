using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface IWorldController
{
    IEnumerable<WorldPoco> GetAllWorlds();
    ActionResult<WorldPoco?> GetWorld(int id);
}