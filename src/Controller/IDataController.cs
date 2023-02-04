using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface IDataController<T> where T : class
{
    [HttpGet]
    IEnumerable<T> GetAll();

    [HttpGet("{id}")]
    T? Get(int id);
}
