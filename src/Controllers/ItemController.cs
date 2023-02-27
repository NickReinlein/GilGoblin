using System.Collections.Generic;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Controllers;

[ApiController]
[Route("[Controller]")]
public class ItemController : ControllerBase, IDataController<ItemInfoPoco>
{
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemRepository itemRepo, ILogger<ItemController> logger)
    {
        _itemRepo = itemRepo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemInfoPoco>> GetAll()
    {
        _logger.LogInformation($"Fetching all item info data");
        return await _itemRepo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ItemInfoPoco?> Get(int id)
    {
        _logger.LogInformation($"Fetching item info id: {id}");
        return await _itemRepo.Get(id);
    }
}
