using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceSavingTests : SaveEntityToDbTests<PricePoco>
{
    private const int itemId = 13245;
    private AverageSalePricePoco _averageSalePrice;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        await using var ctx = GetDbContext();
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
        AverageSalePriceId: _averageSalePrice.Id)
    {
        AverageSalePrice = _averageSalePrice
    };

    protected override PricePoco GetModifiedEntity(PricePoco entity) =>
        entity with
        {
            AverageSalePrice = _averageSalePrice with
            {
                DcDataPoint = _averageSalePrice.DcDataPoint! with
                {
                    Price = _averageSalePrice.DcDataPoint.Price + 1
                }
            }
        };

    protected override async Task ValidateResultSavedToDatabase(PricePoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.Price.FirstOrDefaultAsync(x =>
            x.ItemId == entity.ItemId &&
            x.WorldId == entity.WorldId &&
            x.IsHq == entity.IsHq);
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
            existing.AverageSalePrice = entity.AverageSalePrice;
            existing.RecentPurchase = entity.RecentPurchase;
            existing.MinListing = entity.MinListing;

            ctx.Update(existing);
        }
        else
            await ctx.AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }
}