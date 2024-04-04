using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface ICraftController<T> where T : class
{
    [HttpGet("")]
    Task<ActionResult<List<T>>> GetBestAsync(int worldId);

    [HttpGet("{recipeId:int}")]
    Task<ActionResult<T>> GetAsync(int worldId, int recipeId);
}