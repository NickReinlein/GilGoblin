using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controllers;

public interface IDataController<T>
    where T : class
{
    [HttpGet]
    IEnumerable<T> GetAll();

    [HttpGet("{id}")]
    T? Get(int id);
}
