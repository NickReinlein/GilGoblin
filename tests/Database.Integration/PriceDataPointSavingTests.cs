using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataPointSavingTests : GilGoblinDatabaseFixture
{
    [Test]
    public async Task GivenValidAverageSalePricePoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var averageSalePrice = new AverageSalePricePoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.AverageSalePrice.AddAsync(averageSalePrice);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.AverageSalePrice.SingleAsync(
            x => x.ItemId == averageSalePrice.ItemId &&
                 x.IsHq == averageSalePrice.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ItemId, Is.EqualTo(averageSalePrice.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(averageSalePrice.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(averageSalePrice.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(averageSalePrice.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(averageSalePrice.RegionDataPointId));
        });
    }

    [Test]
    public async Task GivenValidRecentPurchasePocoPoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var recentPurchasePoco = new RecentPurchasePoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.RecentPurchase.AddAsync(recentPurchasePoco);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.RecentPurchase.SingleAsync(x =>
            x.ItemId == recentPurchasePoco.ItemId &&
            x.IsHq == recentPurchasePoco.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ItemId, Is.EqualTo(recentPurchasePoco.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(recentPurchasePoco.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(recentPurchasePoco.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(recentPurchasePoco.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(recentPurchasePoco.RegionDataPointId));
        });
    }

    [Test]
    public async Task GivenValidMinListingPoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var minListing = new MinListingPoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.MinListing.AddAsync(minListing);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.MinListing.SingleAsync(x =>
            x.ItemId == minListing.ItemId &&
            x.IsHq == minListing.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ItemId, Is.EqualTo(minListing.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(minListing.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(minListing.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(minListing.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(minListing.RegionDataPointId));
        });
    }
}