using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataPointSavingTests : GilGoblinDatabaseFixture
{
    [Test]
    public async Task GivenValidAverageSalePricePoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var averageSalePrice = new AverageSalePricePoco(ValidItemsIds[0], ValidWorldIds[0], true, 300, 401, 503);

        await using var ctx = GetDbContext();
        await ctx.AddAsync(averageSalePrice);
        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));

        var result = await ctx.AverageSalePrice.FirstOrDefaultAsync(
            x => x.ItemId == averageSalePrice.ItemId &&
                 x.WorldId == averageSalePrice.WorldId &&
                 x.IsHq == averageSalePrice.IsHq);

        ValidateResult(result, averageSalePrice);
    }

    [Test]
    public async Task GivenValidRecentPurchasePocoPoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var recentPurchasePoco = new RecentPurchasePoco(ValidItemsIds[0], ValidWorldIds[0], true, 300, 401, 503);

        await using var ctx = GetDbContext();
        await ctx.RecentPurchase.AddAsync(recentPurchasePoco);
        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));

        var result = await ctx.RecentPurchase.FirstOrDefaultAsync(x =>
            x.ItemId == recentPurchasePoco.ItemId &&
            x.WorldId == recentPurchasePoco.WorldId &&
            x.IsHq == recentPurchasePoco.IsHq);

        ValidateResult(result, recentPurchasePoco);
    }

    [Test]
    public async Task GivenValidMinListingPoco_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var minListing = new MinListingPoco(ValidItemsIds[0], ValidWorldIds[0], true, 300, 401, 503);
        await using var ctx = GetDbContext();
        await ctx.MinListing.AddAsync(minListing);
        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));

        var result = await ctx.MinListing.FirstOrDefaultAsync(x =>
            x.ItemId == minListing.ItemId &&
            x.WorldId == minListing.WorldId &&
            x.IsHq == minListing.IsHq);

        ValidateResult(result, minListing);
    }

    private static void ValidateResult(PriceDataPointPoco? result, PriceDataPointPoco expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(expected.ItemId));
            Assert.That(result.WorldId, Is.EqualTo(expected.WorldId));
            Assert.That(result.IsHq, Is.EqualTo(expected.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(expected.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(expected.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(expected.RegionDataPointId));
        });
    }
}