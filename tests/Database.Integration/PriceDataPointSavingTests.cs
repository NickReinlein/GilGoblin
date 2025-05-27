using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class AverageSalePricePocoSavingTests : PriceDataPointSavingTests<AverageSalePricePoco>
{
    protected override AverageSalePricePoco GetEntity() => new(ValidItemsIds[0], ValidWorldIds[0], false);
}

public class RecentPurchasePocoSavingTests : PriceDataPointSavingTests<RecentPurchasePoco>
{
    protected override RecentPurchasePoco GetEntity() => new(ValidItemsIds[0], ValidWorldIds[0], false);
}

public class MinListingPocoSavingTests : PriceDataPointSavingTests<MinListingPoco>
{
    protected override MinListingPoco GetEntity() => new(ValidItemsIds[0], ValidWorldIds[0], false);
}

public abstract class PriceDataPointSavingTests<T> : SaveEntityToDbTests<T> where T : PriceDataPointPoco
{
    [SetUp]
    public async Task SetUp()
    {
        await using var ctx = GetDbContext();
        ctx.AverageSalePrice.RemoveRange(ctx.AverageSalePrice);
        ctx.RecentPurchase.RemoveRange(ctx.RecentPurchase);
        ctx.MinListing.RemoveRange(ctx.MinListing);
        await ctx.SaveChangesAsync();
    }

    protected override T GetEntity() => throw new System.NotImplementedException();

    protected override T GetModifiedEntity(T entity)
    {
        return entity with { DcDataPoint = new PriceDataPoco("DC", 300, 280, 300), };
    }

    protected override async Task ValidateResultSavedToDatabase(T entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Set<T>()
            .AsNoTracking()
            .Include(x => x.DcDataPoint)
            .Include(x => x.RegionDataPoint)
            .Include(x => x.WorldDataPoint)
            .FirstOrDefaultAsync(x =>
                x.ItemId == entity.ItemId &&
                x.IsHq == entity.IsHq &&
                x.WorldId == entity.WorldId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.DcDataPoint, Is.EqualTo(entity.DcDataPoint));
            Assert.That(result.RegionDataPoint, Is.EqualTo(entity.RegionDataPoint));
            Assert.That(result.WorldDataPoint, Is.EqualTo(entity.WorldDataPoint));
        });
    }

    protected override async Task SaveEntityToDatabase(T entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        if (update)
        {
            var existing = await ctx.Set<T>().FirstAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);

            existing.RegionDataPoint = entity.RegionDataPoint;
            existing.DcDataPoint = entity.DcDataPoint;
            existing.WorldDataPoint = entity.WorldDataPoint;

            ctx.Update(existing);
        }
        else
            await ctx.AddAsync(entity);

        await ctx.SaveChangesAsync();
    }
}