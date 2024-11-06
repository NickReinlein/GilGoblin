using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class RecipeProfitSavingTests : SaveEntityToDbTests<RecipeProfitPoco>
{
    protected override RecipeProfitPoco GetEntity() =>
        new(9812, ValidWorldIds[0], false, 311, DateTimeOffset.UtcNow.AddDays(-3));

    protected override RecipeProfitPoco GetModifiedEntity(RecipeProfitPoco entity) =>
        entity with { Amount = entity.Amount + 11, LastUpdated = DateTimeOffset.UtcNow };

    protected override async Task ValidateResultSavedToDatabase(RecipeProfitPoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.RecipeProfit.FirstOrDefaultAsync(
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