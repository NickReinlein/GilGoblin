using GilGoblin.Pocos;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface IPriceController
{
    [HttpGet("{worldId}")]
    IEnumerable<MarketDataPoco> GetAll(int worldID);

    [HttpGet("{worldId}/{id}")]
    MarketDataPoco Get(int worldID, int id);
}
