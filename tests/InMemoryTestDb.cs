using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests;

public class InMemoryTestDb
{
    protected DbContextOptions<TestGilGoblinDbContext> _options;
    protected IConfiguration _configuration;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _configuration = Substitute.For<IConfiguration>();

        _options = Substitute.For<DbContextOptions<TestGilGoblinDbContext>>();
        _options = new DbContextOptionsBuilder<TestGilGoblinDbContext>().Options;
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
        var context = new TestGilGoblinDbContext(_options, _configuration);
        context.Price.RemoveRange(context.Price);
        context.Item.RemoveRange(context.Item);
        context.Recipe.RemoveRange(context.Recipe);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.SaveChanges();
    }

    private void CreateAllEntries()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.RecipeCost.AddRange(
            new RecipeCostPoco { WorldId = 22, RecipeId = 11, Cost = 107 },
            new RecipeCostPoco { WorldId = 22, RecipeId = 12, Cost = 297 },
            new RecipeCostPoco { WorldId = 33, RecipeId = 88, Cost = 224 },
            new RecipeCostPoco { WorldId = 44, RecipeId = 99, Cost = 351 }
        );
        context.Recipe.AddRange(
            new RecipePoco { Id = 11, TargetItemId = 111 },
            new RecipePoco { Id = 22, TargetItemId = 111 },
            new RecipePoco { Id = 33, TargetItemId = 222 },
            new RecipePoco { Id = 44, TargetItemId = 333 }
        );
        context.Price.AddRange(
            new PricePoco { WorldId = 22, ItemId = 11 },
            new PricePoco { WorldId = 22, ItemId = 12 },
            new PricePoco { WorldId = 33, ItemId = 88 },
            new PricePoco { WorldId = 44, ItemId = 99 }
        );
        context.Item.AddRange(
            new ItemPoco { Id = 1, Name = "Item 1" },
            new ItemPoco { Id = 2, Name = "Item 2" },
            new ItemPoco { Id = 3, Name = "Item 3" },
            new ItemPoco { Id = 22, Name = "Item 22" }
        );
        context.SaveChanges();
    }
}