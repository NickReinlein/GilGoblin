using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface IPriceController
{
    [HttpGet("{worldId:int}/{id:int}/{isHq:bool}")]
    PricePoco? Get(int worldId, int id, bool isHq);

    [HttpGet("{worldId:int}")]
    IEnumerable<PricePoco> GetAll(int worldId);
}
