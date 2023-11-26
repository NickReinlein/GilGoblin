using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Api.Controllers;

public interface ICraftController<T> where T : class
{
    [HttpGet("{world}")]
    Task<IEnumerable<T>> GetBestCrafts(int worldId);
}