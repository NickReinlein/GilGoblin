using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class AverageSalePricePocoSavingTests : PriceDataPointSavingTests<AverageSalePricePoco>
{
    protected override AverageSalePricePoco GetEntity() => new(12345, ValidWorldIds[0], false);
}

public class RecentPurchasePocoPocoSavingTests : PriceDataPointSavingTests<RecentPurchasePoco>
{
    protected override RecentPurchasePoco GetEntity() => new(12345, ValidWorldIds[0], false);
}

public class MinListingPocoPocoSavingTests : PriceDataPointSavingTests<MinListingPoco>
{
    protected override MinListingPoco GetEntity() => new(12345, ValidWorldIds[0], false);
}

public abstract class PriceDataPointSavingTests<T> : SaveEntityToDbTests<T> where T : PriceDataPointPoco
{
    [SetUp]
    public override async Task SetUp()
    {
        await using var ctx = GetDbContext();
        ctx.AverageSalePrice.RemoveRange(ctx.AverageSalePrice);
        ctx.RecentPurchase.RemoveRange(ctx.RecentPurchase);
        ctx.MinListing.RemoveRange(ctx.MinListing);
        await ctx.SaveChangesAsync();
    }

    protected override T GetModifiedEntity(T entity)
    {
        return entity with
        {
            DcDataPointId = entity.DcDataPointId + 11,
            RegionDataPointId = entity.RegionDataPointId + 11,
            WorldDataPointId = entity.WorldDataPointId + 11
        };
    }

    // protected override async Task<T> SavePocoToDatabase(T entity, bool update = false)
    // {
    //     await using var ctx = GetDbContext();
    //     if (update)
    //     {
    //         var existing = await ctx.Set<T>().FirstAsync(x =>
    //             x.ItemId == entity.ItemId &&
    //             x.WorldId == entity.WorldId &&
    //             x.IsHq == entity.IsHq);
    //
    //         ctx.Update(existing);
    //     }
    //     else
    //         await ctx.AddAsync(entity);
    //
    //     var savedCount = await ctx.SaveChangesAsync();
    //     Assert.That(savedCount, Is.EqualTo(1));
    //
    //     return entity;
    // }

    protected override async Task ValidateResultSavedToDatabaseAsync(T entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Set<T>().FirstOrDefaultAsync(
            x => x.ItemId == entity.ItemId &&
                 x.IsHq == entity.IsHq &&
                 x.WorldId == entity.WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.DcDataPointId, Is.EqualTo(entity.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(entity.RegionDataPointId));
            Assert.That(result.WorldDataPointId, Is.EqualTo(entity.WorldDataPointId));
        });
    }
}