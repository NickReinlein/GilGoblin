using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface IPriceController
{
    [HttpGet("{worldId:int}")]
    IEnumerable<PricePoco> GetAll(int worldId);

    [HttpGet("{worldId:int}/{id:int}")]
    PricePoco? Get(int worldId, int id);
}
