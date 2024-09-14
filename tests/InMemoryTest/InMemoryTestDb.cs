using System;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.InMemoryTest;

public class InMemoryTestDb
{
    protected DbContextOptions<TestGilGoblinDbContext> _options;
    protected IConfiguration _configuration;

    protected static readonly List<int> ValidWorldIds = [34, 99];

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
        DeleteAllEntries();
        CreateAllEntries();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DeleteAllEntries();
    }

    private void DeleteAllEntries()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.RecentPurchase.RemoveRange(context.RecentPurchase);
        context.AverageSalePrice.RemoveRange(context.AverageSalePrice);
        context.MinListing.RemoveRange(context.MinListing);
        context.DailySaleVelocity.RemoveRange(context.DailySaleVelocity);
        context.World.RemoveRange(context.World);
        context.Item.RemoveRange(context.Item);
        context.Recipe.RemoveRange(context.Recipe);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.RecipeProfit.RemoveRange(context.RecipeProfit);
        context.Price.RemoveRange(context.Price);
        context.WorldUploadTime.RemoveRange(context.WorldUploadTime);
        context.SaveChanges();
    }

    private void CreateAllEntries()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        // var pricePoint = new PriceDataPointPoco(1, 11, true);
        context.Price.AddRange(
            new PricePoco { Id = 1, WorldId = 34, IsHq = true, Updated = DateTimeOffset.Now },
            new PricePoco { Id = 2, WorldId = 34, IsHq = false, Updated = DateTimeOffset.Now }
        );
        context.Recipe.AddRange(
            new RecipePoco { Id = 11, TargetItemId = 5682, ItemIngredient0TargetId = 774, AmountIngredient0 = 3 },
            new RecipePoco { Id = 12, TargetItemId = 9984, ItemIngredient0TargetId = 12, AmountIngredient0 = 5 },
            new RecipePoco { Id = 13, TargetItemId = 111, ItemIngredient0TargetId = 14, AmountIngredient0 = 2 },
            new RecipePoco { Id = 33, TargetItemId = 222, ItemIngredient0TargetId = 88, AmountIngredient0 = 7 },
            new RecipePoco { Id = 44, TargetItemId = 333, ItemIngredient0TargetId = 99, AmountIngredient0 = 2 },
            new RecipePoco { Id = 55, TargetItemId = 88, ItemIngredient0TargetId = 3, AmountIngredient0 = 3 },
            new RecipePoco { Id = 88, TargetItemId = 88, ItemIngredient0TargetId = 1, AmountIngredient0 = 3 },
            new RecipePoco { Id = 99, TargetItemId = 9984, ItemIngredient0TargetId = 2, AmountIngredient0 = 5 }
        );
        context.Item.AddRange(
            new ItemPoco { Id = 1, Name = "Item 1" },
            new ItemPoco { Id = 2, Name = "Item 2" },
            new ItemPoco { Id = 3, Name = "Item 3" },
            new ItemPoco { Id = 22, Name = "Item 22" }
        );
        context.World.AddRange(
            new WorldPoco { Id = 34, Name = "World 34" },
            new WorldPoco { Id = 99, Name = "World 99" }
        );
        context.SaveChanges();
    }
}