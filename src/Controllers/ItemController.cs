using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase, IDataController<ItemInfoPoco>
{
    private readonly IItemRepository _itemRepo;
    private readonly ILogger<ItemController> _logger;

    public ItemController(IItemRepository itemRepo, ILogger<ItemController> logger)
    {
        _itemRepo = itemRepo;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ItemInfoPoco? Get(int id)
    {
        _logger.LogInformation($"Fetching item info id: {id}");
        return _itemRepo.Get(id);
    }

    [HttpGet]
    public IEnumerable<ItemInfoPoco> GetAll()
    {
        _logger.LogInformation($"Fetching all item info data");
        return _itemRepo.GetAll();
    }
}
