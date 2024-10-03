using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class DailySaleVelocityTests : GilGoblinDatabaseFixture
{
    [Test]
    public async Task GivenDailySaleVelocityPoco_IsValid_WhenSaving_ThenObjectIsSavedSuccessfully()
    {
        var dailySaleVelocity = new DailySaleVelocityPoco(
            100,
            true,
            new SaleQuantity(100),
            new SaleQuantity(200),
            new SaleQuantity(300));

        await using var ctx = GetNewDbContext();
        await ctx.DailySaleVelocity.AddAsync(dailySaleVelocity);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.DailySaleVelocity.FirstOrDefaultAsync(
            x =>
                x.ItemId == dailySaleVelocity.ItemId &&
                x.IsHq == dailySaleVelocity.IsHq);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(dailySaleVelocity.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(dailySaleVelocity.IsHq));
            Assert.That(result.World, Is.EqualTo(dailySaleVelocity.World));
            Assert.That(result.Dc, Is.EqualTo(dailySaleVelocity.Dc));
            Assert.That(result.Region, Is.EqualTo(dailySaleVelocity.Region));
        });
    }
}