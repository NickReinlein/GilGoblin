using System;
using System.Collections.Generic;
using System.Linq;
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
        var timestamp = DateTimeOffset.UtcNow;
        context.RecipeCost.AddRange(new List<RecipeCostPoco>
        {
            new() { WorldId = 34, RecipeId = 11, Cost = 107, Updated = timestamp },
            new() { WorldId = 34, RecipeId = 12, Cost = 75, Updated = timestamp },
            new() { WorldId = 34, RecipeId = 8854, Cost = 351, Updated = timestamp },
            new() { WorldId = 34, RecipeId = 9841, Cost = 3001, Updated = timestamp }
        });
        context.RecipeProfit.AddRange(
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 11,
                RecipeProfitVsListings = 107,
                RecipeProfitVsSold = 94,
                Updated = DateTimeOffset.UtcNow
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 12,
                RecipeProfitVsListings = 297,
                RecipeProfitVsSold = 254,
                Updated = DateTimeOffset.UtcNow
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 88,
                RecipeProfitVsListings = 224,
                RecipeProfitVsSold = 218,
                Updated = DateTimeOffset.UtcNow
            },
            new RecipeProfitPoco
            {
                WorldId = 34,
                RecipeId = 99,
                RecipeProfitVsListings = 351,
                RecipeProfitVsSold = 317,
                Updated = DateTimeOffset.UtcNow
            }
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
                ItemId = 5682,
                AverageSold = 6557f,
                AverageListingPrice = 3748f,
                LastUploadTime = unixEpochTimestamp
            },
            new PricePoco
            {
                WorldId = 34,
                ItemId = 9984,
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
        context.SaveChanges();

        var itemIds = new List<int>
        {
            1,
            2,
            3,
            11,
            12,
            88,
            9984
        };
        var recipeTargetItemIds = context.Recipe.Select(r => r.TargetItemId).ToList();
        itemIds.AddRange(recipeTargetItemIds);
        var ids = itemIds.Distinct().ToList();
        foreach (var id in ids)
        {
            context.Item.Add(new ItemPoco { Id = id, Name = $"Item {id}" });
        }
        context.SaveChanges();
    }
}