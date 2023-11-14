using System;
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
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        context.Price.RemoveRange(context.Price);
        context.Item.RemoveRange(context.Item);
        context.Recipe.RemoveRange(context.Recipe);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.RecipeProfit.RemoveRange(context.RecipeProfit);
        context.SaveChanges();
    }

    private void CreateAllEntries()
    {
        using var context = new TestGilGoblinDbContext(_options, _configuration);
        var unixEpochTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        context.RecipeCost.AddRange(
            new RecipeCostPoco { WorldId = 34, RecipeId = 11, Cost = 107, Updated = DateTimeOffset.Now },
            new RecipeCostPoco { WorldId = 34, RecipeId = 12, Cost = 297, Updated = DateTimeOffset.Now },
            new RecipeCostPoco { WorldId = 34, RecipeId = 88, Cost = 224, Updated = DateTimeOffset.Now },
            new RecipeCostPoco { WorldId = 34, RecipeId = 99, Cost = 351, Updated = DateTimeOffset.Now }
        );
        context.RecipeProfit.AddRange(
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 11,
                RecipeProfitVsListings = 107,
                RecipeProfitVsSold = 94,
                Updated = DateTimeOffset.Now
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 12,
                RecipeProfitVsListings = 297,
                RecipeProfitVsSold = 254,
                Updated = DateTimeOffset.Now
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 88,
                RecipeProfitVsListings = 224,
                RecipeProfitVsSold = 218,
                Updated = DateTimeOffset.Now
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 99,
                RecipeProfitVsListings = 351,
                RecipeProfitVsSold = 317,
                Updated = DateTimeOffset.Now
            }
        );
        context.Recipe.AddRange(
            new RecipePoco { Id = 11, TargetItemId = 111, ItemIngredient0TargetId = 11, AmountIngredient0 = 3 },
            new RecipePoco { Id = 22, TargetItemId = 111, ItemIngredient0TargetId = 12, AmountIngredient0 = 5 },
            new RecipePoco { Id = 33, TargetItemId = 222, ItemIngredient0TargetId = 88, AmountIngredient0 = 7 },
            new RecipePoco { Id = 44, TargetItemId = 333, ItemIngredient0TargetId = 99, AmountIngredient0 = 2 }
        );
        context.Price.AddRange(
            new PricePoco
            {
                WorldId = 34,
                ItemId = 11,
                AverageSold = 133f,
                AverageListingPrice = 241f,
                LastUploadTime = unixEpochTimestamp
            },
            new PricePoco
            {
                WorldId = 34,
                ItemId = 12,
                AverageSold = 245f,
                AverageListingPrice = 377f,
                LastUploadTime = unixEpochTimestamp
            },
            new PricePoco
            {
                WorldId = 34,
                ItemId = 88,
                AverageSold = 247f,
                AverageListingPrice = 361f,
                LastUploadTime = unixEpochTimestamp
            },
            new PricePoco
            {
                WorldId = 34,
                ItemId = 99,
                AverageSold = 387f,
                AverageListingPrice = 477f,
                LastUploadTime = unixEpochTimestamp
            },
            new PricePoco
            {
                WorldId = 34,
                ItemId = 9001,
                AverageSold = 1011f,
                AverageListingPrice = 1051f,
                LastUploadTime = unixEpochTimestamp
            }
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