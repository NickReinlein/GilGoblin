using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class DailySaleVelocitySavingTests : SaveEntityToDbTests<DailySaleVelocityPoco>
{
    protected override DailySaleVelocityPoco GetEntity() =>
        new(1232,
            ValidWorldIds[0],
            true,
            new SaleQuantity(100),
            new SaleQuantity(200),
            new SaleQuantity(300));

    protected override DailySaleVelocityPoco GetModifiedEntity(DailySaleVelocityPoco entity) =>
        new(entity.ItemId + 11,
            entity.WorldId,
            !entity.IsHq,
            entity.World with { Quantity = entity.World?.Quantity ?? 0 + 11 },
            entity.Dc with { Quantity = entity.Dc?.Quantity ?? 0 + 11 },
            entity.Region with { Quantity = entity.Region?.Quantity ?? 0 + 11 });

    protected override async Task ValidateResultSavedToDatabase(DailySaleVelocityPoco entity)
    {
        await using var ctx = GetDbContext();

        var result = await ctx.DailySaleVelocity.FirstOrDefaultAsync(
            x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.World, Is.EqualTo(entity.World));
            Assert.That(result.Dc, Is.EqualTo(entity.Dc));
            Assert.That(result.Region, Is.EqualTo(entity.Region));
        });
    }

    protected override async Task SavePocoToDatabase(DailySaleVelocityPoco entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        if (update)
        {
            var existing = await ctx.DailySaleVelocity.FirstAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);
            existing.Region = entity.Region;
            existing.Dc = entity.Dc;
            existing.World = entity.World;

            ctx.Update(existing);
        }
        else
            await ctx.Set<DailySaleVelocityPoco>().AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }
}