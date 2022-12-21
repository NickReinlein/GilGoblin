using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface ICraftController<T> where T : class
{
    [HttpGet("{world}/{id}")]
    T Get(int worldId, int id);
}
