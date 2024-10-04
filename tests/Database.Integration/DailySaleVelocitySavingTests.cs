using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class DailySaleVelocitySavingTests : SaveEntityToDbTests<DailySaleVelocityPoco>
{
    protected override DailySaleVelocityPoco GetEntity() =>
        new(100,
            true,
            new SaleQuantity(100),
            new SaleQuantity(200),
            new SaleQuantity(300));

    protected override DailySaleVelocityPoco GetModifiedEntity(DailySaleVelocityPoco entity) =>
        new(entity.ItemId + 11,
            !entity.IsHq,
            new SaleQuantity(entity.World?.Quantity ?? 0 + 11),
            new SaleQuantity(entity.Dc?.Quantity ?? 0 + 11),
            new SaleQuantity(entity.Region?.Quantity ?? 0 + 11));

    protected override async Task ValidateResultSavedToDatabaseAsync(DailySaleVelocityPoco entity)
    {
        await using var ctx = GetNewDbContext();

        var result = await ctx.DailySaleVelocity.FirstOrDefaultAsync(
            x =>
                x.ItemId == entity.ItemId &&
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
}