using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceSavingTests : SaveEntityToDbTests<PricePoco>
{
    protected override PricePoco GetEntity() =>
        new(111, 222, false, DateTimeOffset.UtcNow);

    protected override PricePoco GetModifiedEntity(PricePoco entity) =>
        new(entity.ItemId, entity.WorldId, entity.IsHq, DateTimeOffset.UtcNow);

    protected override async Task ValidateResultSavedToDatabaseAsync(PricePoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Price.FirstOrDefaultAsync(
            x => x.ItemId == entity.ItemId &&
                 x.IsHq == entity.IsHq &&
                 x.WorldId == entity.WorldId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.Updated, Is.GreaterThan(DateTimeOffset.UtcNow.AddSeconds(-1)));
        });
    }
}