using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface IGilGoblinController<T> where T : class
{
    [HttpGet]
    IEnumerable<T> Get();

    [HttpGet("{id}")]
    T Get(int id);
}
