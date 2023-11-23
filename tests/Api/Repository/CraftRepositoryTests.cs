using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository;

public class CraftRepositoryTests
{
    private CraftRepository _craftRepository;

    private ICraftingCalculator _calc;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IRecipeCostRepository _recipeCostRepository;
    private IRecipeProfitRepository _recipeProfitRepository;
    private IItemRepository _itemRepository;
    private ICraftCache _cache;
    private ILogger<CraftRepository> _logger;
    private readonly int _worldId = 22;
    private readonly int _itemId = 6400;
    private readonly int _recipeId = 444;
    private readonly int _recipeCost = 777;
    private readonly string _itemName = "Excalibur";

    

    [SetUp]
    public void SetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _calc.CalculateCraftingCostForItem(_worldId, _itemId).Returns((_recipeId, _recipeCost));

        _priceRepository = Substitute.For<IPriceRepository<PricePoco>>();
        _priceRepository
            .Get(_worldId, _itemId)
            .Returns(new PricePoco { WorldId = _worldId, ItemId = _itemId });

        _recipeRepository = Substitute.For<IRecipeRepository>();
        _recipeRepository.Get(_recipeId).Returns(new RecipePoco { Id = _recipeId, TargetItemId = _itemId });
        _recipeRepository.GetAll().Returns(GetCrafts());

        _recipeCostRepository = Substitute.For<IRecipeCostRepository>();
        _recipeProfitRepository = Substitute.For<IRecipeProfitRepository>();

        _itemRepository = Substitute.For<IItemRepository>();
        _itemRepository.Get(_itemId).Returns(new ItemPoco { Id = _itemId, Name = _itemName });

        _cache = Substitute.For<ICraftCache>();
        _cache
            .Get((_worldId, _itemId))
            .Returns(null,
                new CraftSummaryPoco { ItemId = _itemId, ItemInfo = new ItemPoco { Id = _itemId, Name = _itemName } });

        _logger = Substitute.For<ILogger<CraftRepository>>();

        _craftRepository = new CraftRepository(
            _priceRepository,
            _recipeRepository,
            _recipeCostRepository,
            _recipeProfitRepository,
            _itemRepository,
            _cache,
            _logger
        );
    }

    private List<RecipePoco> GetCrafts() => new()
    {
        new() { Id = _recipeId, TargetItemId = _itemId }, new() { Id = _recipeId + 1, TargetItemId = _itemId + 1 }
    };
}