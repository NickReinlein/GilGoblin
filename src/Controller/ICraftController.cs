using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface ICraftController<T> where T : class
{
    [HttpGet("{world}/{id}")]
    T? GetCraft(int worldId, int id);

    [HttpGet("{world}")]
    IEnumerable<T?> GetBestCrafts(int worldId);
}
