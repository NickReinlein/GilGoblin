using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceSavingTests : SaveEntityToDbTests<PricePoco>
{
    private const int itemId = 13245;
    private AverageSalePricePoco _averageSalePrice;

    [SetUp]
    public async Task SetUp()
    {
        await using var ctx = GetDbContext();
        ctx.Price.RemoveRange(ctx.Price);
        await ctx.SaveChangesAsync();
        var dcDataPoint = new PriceDataPoco("DC", 311, ValidWorldIds[0]);
        ctx.Add(dcDataPoint);
        var saved = await ctx.SaveChangesAsync();
        Assert.That(saved, Is.EqualTo(1));
        _averageSalePrice = new AverageSalePricePoco(itemId, ValidWorldIds[0], false, dcDataPoint.Id)
        {
            DcDataPoint = dcDataPoint
        };
        ctx.Add(_averageSalePrice);
        var result = await ctx.SaveChangesAsync();
        Assert.That(result, Is.EqualTo(1));
    }

    protected override PricePoco GetEntity() => new(
        13245,
        ValidWorldIds[0],
        false,
        AverageSalePriceId: _averageSalePrice.Id) { AverageSalePrice = _averageSalePrice };

    protected override PricePoco GetModifiedEntity(PricePoco entity) =>
        entity with
        {
            Id = entity.Id,
            AverageSalePrice = _averageSalePrice with
            {
                DcDataPoint = _averageSalePrice.DcDataPoint! with
                {
                    Price = entity.AverageSalePrice?.DcDataPoint?.Price ?? 0 + 1
                }
            }
        };

    protected override async Task ValidateResultSavedToDatabase(PricePoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Price
            .AsNoTracking()
            .Include(x => x.AverageSalePrice)
            .Include(x => x.AverageSalePrice!.DcDataPoint)
            .FirstOrDefaultAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(entity.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(entity.IsHq));
            Assert.That(result.Updated, Is.GreaterThan(entity.Updated.AddSeconds(-3)));
            Assert.That(result.AverageSalePriceId, Is.EqualTo(entity.AverageSalePriceId));
            Assert.That(result.AverageSalePrice?.DcDataPoint?.Price,
                Is.EqualTo(entity.AverageSalePrice?.DcDataPoint?.Price));
        });
    }

    protected override async Task SaveEntityToDatabase(PricePoco entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        ctx.Entry(entity.AverageSalePrice!).State = EntityState.Unchanged;
        ctx.Entry(entity.AverageSalePrice!.DcDataPoint!).State = EntityState.Unchanged;
        if (update)
        {
            var existing = await ctx.Price.FirstAsync(x =>
                x.ItemId == entity.ItemId &&
                x.WorldId == entity.WorldId &&
                x.IsHq == entity.IsHq);
            existing.Updated = entity.Updated;
            existing.AverageSalePrice = entity.AverageSalePrice;
            existing.RecentPurchase = entity.RecentPurchase;
            existing.MinListing = entity.MinListing;
            existing.DailySaleVelocity = entity.DailySaleVelocity;

            ctx.Update(existing);
        }
        else
            await ctx.AddAsync(entity);

        await ctx.SaveChangesAsync();
    }
}