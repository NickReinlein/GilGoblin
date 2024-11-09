using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceSavingTests : SaveEntityToDbTests<PricePoco>
{
    protected override PricePoco GetEntity() =>
        new(35445, ValidWorldIds[0], false, 311);

    protected override PricePoco GetModifiedEntity(PricePoco entity) =>
        entity with { MinListingId = (entity.MinListingId ?? 0) + 11 };

    protected override async Task ValidateResultSavedToDatabase(PricePoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Price.FirstOrDefaultAsync(x =>
            x.ItemId == entity.ItemId &&
            x.IsHq == entity.IsHq &&
            x.WorldId == entity.WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.Updated, Is.GreaterThan(DateTimeOffset.UtcNow.AddSeconds(-10)));
        });
    }

    protected override async Task SaveEntityToDatabase(PricePoco entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        if (update)
        {
            var existing = await ctx.Price.FirstAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);
            existing.Updated = entity.Updated;

            ctx.Update(existing);
        }
        else
            await ctx.Set<PricePoco>().AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }
}