// using GilGoblin.Crafting;
// using GilGoblin.Database;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NUnit.Framework;

// namespace GilGoblin.Tests.Database;

// public class CraftRepositoryComponentTests : InMemoryTestDb
// {
//     private CraftRepository _craftRepository;

//     private ICraftingCalculator _calc;
//     private IPriceRepository<PricePoco> _priceRepository;
//     private IRecipeRepository _recipeRepository;
//     private IItemRepository _itemRepository;
//     private ILogger<CraftRepository> _logger;

//     public static readonly int WorldID = 22;
//     public static readonly int ItemID = 6400;
//     public static readonly int RecipeID = 444;
//     public static readonly float CraftingCost = 777;
//     public static readonly string ItemName = "Excalibur";

//     [OneTimeSetUp]
//     public override void OneTimeSetUp()
//     {
//         base.OneTimeSetUp();

//         using var context = new GilGoblinDbContext(_options, _configuration);
//         context.Recipe.AddRange(
//             new RecipePoco { ID = 111, TargetItemID = 11 },
//             new RecipePoco { ID = 112, TargetItemID = 11 },
//             new RecipePoco { ID = 888, TargetItemID = 88 },
//             new RecipePoco { ID = 999, TargetItemID = 99 }
//         );
//         context.ItemInfo.AddRange(
//             new ItemInfoPoco { ID = 11, Name = "Item 11" },
//             new ItemInfoPoco { ID = 12, Name = "Item 12" },
//             new ItemInfoPoco { ID = 88, Name = "Item 88" },
//             new ItemInfoPoco { ID = 99, Name = "Item 99" }
//         );
//         context.Price.AddRange(
//             new PricePoco { WorldID = WorldID, ItemID = 11 },
//             new PricePoco { WorldID = WorldID, ItemID = 12 },
//             new PricePoco { WorldID = 33, ItemID = 88 },
//             new PricePoco { WorldID = 44, ItemID = 99 }
//         );
//         context.SaveChanges();
//     }
// }
