using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

public interface IDataController<T> where T : class
{
    [HttpGet]
    Task<IEnumerable<T?>> GetAll();

    [HttpGet("{id}")]
    Task<IEnumerable<T?>> Get(int id);
}
