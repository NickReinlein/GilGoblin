using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class RecipeCostSavingTests : SaveEntityToDbTests<RecipeCostPoco>
{
    protected override RecipeCostPoco GetEntity() =>
        new(9812, ValidWorldIds[0], false, 311, DateTimeOffset.UtcNow.AddDays(-3));

    protected override RecipeCostPoco GetModifiedEntity(RecipeCostPoco entity) =>
        entity with { Amount = entity.Amount + 11, LastUpdated = DateTimeOffset.UtcNow };

    protected override async Task ValidateResultSavedToDatabase(RecipeCostPoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.RecipeCost
            .AsNoTracking()
            .FirstOrDefaultAsync(
            x =>
                x.RecipeId == entity.RecipeId &&
                x.IsHq == entity.IsHq &&
                x.WorldId == entity.WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.RecipeId, Is.EqualTo(entity.RecipeId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.LastUpdated, Is.GreaterThan(entity.LastUpdated.AddSeconds(-1)));
        });
    }
}