using GilGoblin.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controllers;

public interface IPriceController
{
    [HttpGet("{worldId}")]
    IEnumerable<PricePoco> GetAll(int worldId);

    [HttpGet("{worldId}/{id}")]
    PricePoco? Get(int worldId, int id);
}
