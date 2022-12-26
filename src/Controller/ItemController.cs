using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase, IDataController<ItemInfoPoco>
{
    private readonly ILogger<ItemController> _logger;

    public ItemController(ILogger<ItemController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<ItemInfoPoco> GetAll()
    {
        _logger.LogInformation($"Fetching all item info data");
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }

    [HttpGet("{id}")]
    public ItemInfoPoco Get(int id)
    {
        _logger.LogInformation($"Fetching item info id: {id}");
        return new ItemInfoPoco
        {
            ID = id,
            Description = "TestItem" + id,
            GatheringID = id + 144,
            IconID = id + 42,
            Name = "TestItem" + id,
            StackSize = 1,
            VendorPrice = id
        };
    }
}
