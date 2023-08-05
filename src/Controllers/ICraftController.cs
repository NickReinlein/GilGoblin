using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controllers;

public interface ICraftController<T>
    where T : class
{
    [HttpGet("{world}/{id}")]
    Task<T?> GetCraft(int worldId, int id);

    [HttpGet("{world}")]
    IEnumerable<T> GetBestCrafts(int worldId);
}
