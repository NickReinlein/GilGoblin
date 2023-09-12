using GilGoblin.Database;
using GilGoblin.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests;

public class InMemoryTestDb
{
    protected DbContextOptions<GilGoblinDbContext> _options;
    protected IConfiguration _configuration;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _configuration = Substitute.For<IConfiguration>();

        _options = Substitute.For<DbContextOptions<GilGoblinDbContext>>();
        _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
            .UseInMemoryDatabase(databaseName: "GilGoblinTest")
            .Options;
    }

    [SetUp]
    public virtual void SetUp()
    {
        CreateAllEntries();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DeleteAllEntries();
    }

    private void DeleteAllEntries()
    {
        var context = new GilGoblinDbContext(_options, _configuration);
        context.Price.RemoveRange(context.Price);
        context.ItemInfo.RemoveRange(context.ItemInfo);
        context.Recipe.RemoveRange(context.Recipe);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.SaveChanges();
    }

    private void CreateAllEntries()
    {
        using var context = new GilGoblinDbContext(_options, _configuration);
        context.RecipeCost.AddRange(
            new RecipeCostPoco { WorldID = 22, RecipeID = 11 },
            new RecipeCostPoco { WorldID = 22, RecipeID = 12 },
            new RecipeCostPoco { WorldID = 33, RecipeID = 88 },
            new RecipeCostPoco { WorldID = 44, RecipeID = 99 }
        );
        context.Recipe.AddRange(
            new RecipePoco { ID = 11, TargetItemID = 111 },
            new RecipePoco { ID = 22, TargetItemID = 111 },
            new RecipePoco { ID = 33, TargetItemID = 222 },
            new RecipePoco { ID = 44, TargetItemID = 333 }
        );
        context.Price.AddRange(
            new PricePoco { WorldID = 22, ItemID = 11 },
            new PricePoco { WorldID = 22, ItemID = 12 },
            new PricePoco { WorldID = 33, ItemID = 88 },
            new PricePoco { WorldID = 44, ItemID = 99 }
        );
        context.ItemInfo.AddRange(
            new ItemInfoPoco { ID = 1, Name = "Item 1" },
            new ItemInfoPoco { ID = 2, Name = "Item 2" },
            new ItemInfoPoco { ID = 3, Name = "Item 3" },
            new ItemInfoPoco { ID = 22, Name = "Item 22" }
        );
        context.SaveChanges();
    }
}
