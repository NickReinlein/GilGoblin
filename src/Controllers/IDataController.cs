using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controllers;

public interface IDataController<T> where T : class
{
    [HttpGet]
    Task<IEnumerable<T>> GetAll();

    [HttpGet("{id}")]
    Task<T?> Get(int id);
}
