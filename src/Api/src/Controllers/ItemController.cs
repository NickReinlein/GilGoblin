using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase, IDataController<ItemPoco>
{
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemRepository itemRepo, ILogger<ItemController> logger)
    {
        _itemRepo = itemRepo;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ItemPoco? Get(int id)
    {
        _logger.LogInformation($"Fetching item info id: {id}");
        return _itemRepo.Get(id);
    }

    [HttpGet]
    public IEnumerable<ItemPoco> GetAll()
    {
        _logger.LogInformation($"Fetching all item info data");
        return _itemRepo.GetAll();
    }
}
