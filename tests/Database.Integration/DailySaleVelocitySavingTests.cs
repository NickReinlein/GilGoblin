using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class DailySaleVelocitySavingTests : SaveEntityToDbTests<DailySaleVelocityPoco>
{
    protected override DailySaleVelocityPoco GetEntity() =>
        new(ValidItemsIds[0],
            ValidWorldIds[0],
            true,
            111,
            131,
            271);

    protected override DailySaleVelocityPoco GetModifiedEntity(DailySaleVelocityPoco entity) =>
        entity with
        {
            ItemId = entity.ItemId,
            WorldId = entity.WorldId,
            IsHq = entity.IsHq,
            World = entity.World ?? 0 + 11,
            Dc = entity.Dc ?? 0 + 11,
            Region = entity.Region ?? 0 + 11
        };

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

    protected override async Task SaveEntityToDatabase(DailySaleVelocityPoco entity, bool update = false)
    {
        await using var dbContext = GetDbContext();
        if (update)
        {
            var existing = await dbContext.DailySaleVelocity.FirstAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);

            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }
        else
            await dbContext.Set<DailySaleVelocityPoco>().AddAsync(entity);

        var savedCount = await dbContext.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }
}