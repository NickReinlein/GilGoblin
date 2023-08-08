using GilGoblin.Crafting;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class CraftRepositoryTests : InMemoryTestDb
{
    private CraftRepository _craftRepository;

    private ICraftingCalculator _calc;
    private IPriceRepository<PricePoco> _priceRepository;
    private IRecipeRepository _recipeRepository;
    private IItemRepository _itemRepository;
    private ILogger<CraftRepository> _logger;

    [Test, Ignore("Re-enable when GetBestCraft is complete")]
    public void GivenAGetBestCrafts_WhenTheWorldIdIsValid_ThenTheRepositoryReturnsBestCrafts()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);

        var result = _craftRepository.GetBestCrafts(22);

        Assert.That(result, Is.Empty);
    }

    // [Test]
    // public void GivenAGetAll_ThenTheRepositoryReturnsAllEntries()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);

    //     var result = recipeRepo.GetAll();

    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.EqualTo(4));
    //         Assert.That(result.Any(p => p.ID == 11));
    //         Assert.That(result.Any(p => p.ID == 22));
    //         Assert.That(result.Any(p => p.ID == 33));
    //         Assert.That(result.Any(p => p.ID == 44));
    //     });
    // }

    // [TestCase(11)]
    // [TestCase(22)]
    // [TestCase(33)]
    // [TestCase(44)]
    // public void GivenAGet_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.Get(id);

    //     Assert.That(result.ID == id);
    // }

    // [TestCase(111, 2)]
    // [TestCase(222, 1)]
    // [TestCase(333, 1)]
    // public void GivenAGetCraftsForItem_WhenTheIDIsValid_ThenTheRepositoryReturnsTheCorrectEntries(
    //     int targetItemID,
    //     int expectedResultsCount
    // )
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.GetCraftsForItem(targetItemID);

    //     Assert.That(result.Count, Is.EqualTo(expectedResultsCount));
    // }

    // [TestCase(0)]
    // [TestCase(-1)]
    // [TestCase(100)]
    // public void GivenAGet_WhenIDIsInvalid_ThenTheRepositoryReturnsNull(int id)
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.Get(id);

    //     Assert.That(result, Is.Null);
    // }

    // [Test]
    // public void GivenAGetMultiple_WhenIDsAreValid_ThenTheCorrectEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.GetMultiple(new int[] { 11, 33, 44 });

    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.EqualTo(3));
    //         Assert.That(result.Any(p => p.ID == 11));
    //         Assert.That(result.Any(p => p.ID == 33));
    //         Assert.That(result.Any(p => p.ID == 44));
    //     });
    // }

    // [Test]
    // public void GivenAGetMultiple_WhenSomeIDsAreValid_ThenTheValidEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.GetMultiple(new int[] { 22, 857 });

    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Count(), Is.EqualTo(1));
    //         Assert.That(result.Any(p => p.ID == 22));
    //     });
    // }

    // [Test]
    // public void GivenAGetMultiple_WhenIDsAreInvalid_ThenNoEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.GetMultiple(new int[] { 333, 999 });

    //     Assert.That(!result.Any());
    // }

    // [Test]
    // public void GivenAGetMultiple_WhenIDsEmpty_ThenNoEntriesAreReturned()
    // {
    //     using var context = new GilGoblinDbContext(_options, _configuration);
    //     var recipeRepo = new CraftRepository(context);

    //     var result = recipeRepo.GetMultiple(new int[] { });

    //     Assert.That(!result.Any());
    // }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();

        _craftRepository = new CraftRepository(
            _calc,
            _priceRepository,
            _recipeRepository,
            _itemRepository,
            _logger
        );

        using var context = new GilGoblinDbContext(_options, _configuration);
        context.Recipe.AddRange(
            new RecipePoco { ID = 111, TargetItemID = 11 },
            new RecipePoco { ID = 112, TargetItemID = 11 },
            new RecipePoco { ID = 888, TargetItemID = 88 },
            new RecipePoco { ID = 999, TargetItemID = 99 }
        );
        context.ItemInfo.AddRange(
            new ItemInfoPoco { ID = 11, Name = "Item 11" },
            new ItemInfoPoco { ID = 12, Name = "Item 12" },
            new ItemInfoPoco { ID = 88, Name = "Item 88" },
            new ItemInfoPoco { ID = 99, Name = "Item 99" }
        );
        context.Price.AddRange(
            new PricePoco { WorldID = 22, ItemID = 11 },
            new PricePoco { WorldID = 22, ItemID = 12 },
            new PricePoco { WorldID = 33, ItemID = 88 },
            new PricePoco { WorldID = 44, ItemID = 99 }
        );
        context.SaveChanges();
    }
}
