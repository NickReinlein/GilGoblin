using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface ICraftController<T> where T : class
{
    [HttpGet("")]
    Task<IEnumerable<T>> GetBestAsync(int worldId);

    [HttpGet("{recipeId:int}")]
    Task<T?> GetAsync(int worldId, int recipeId);
}