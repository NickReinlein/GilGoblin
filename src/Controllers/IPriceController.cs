using System.Collections.Generic;
using GilGoblin.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controllers;

public interface IPriceController
{
    [HttpGet("{worldId}")]
    IEnumerable<PricePoco> GetAll(int worldID);

    [HttpGet("{worldId}/{id}")]
    PricePoco? Get(int worldID, int id);
}
