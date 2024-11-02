using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataSavingTests : SaveEntityToDbTests<PriceDataPoco>
{
    protected override PriceDataPoco GetEntity() =>
        new("someType", 123m, ValidWorldIds[0], DateTimeOffset.UtcNow.AddDays(-1).UtcTicks);

    protected override PriceDataPoco GetModifiedEntity(PriceDataPoco entity) =>
        new(entity.PriceType, entity.Price, entity.WorldId, DateTimeOffset.UtcNow.UtcTicks);

    protected override async Task ValidateResultSavedToDatabaseAsync(PriceDataPoco entity)
    {
        await using var ctx = GetDbContext();
        var result = await ctx.PriceData.SingleAsync(
            x =>
                x.Id > 0 &&
                x.Price == entity.Price &&
                x.PriceType == entity.PriceType &&
                x.WorldId == entity.WorldId &&
                x.Timestamp == entity.Timestamp);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PriceType, Is.EqualTo(entity.PriceType));
            Assert.That(result.WorldId, Is.EqualTo(entity.WorldId));
            Assert.That(result.Price, Is.EqualTo(entity.Price));
            Assert.That(result.Timestamp, Is.GreaterThan(DateTimeOffset.UtcNow.AddSeconds(-1).UtcTicks));
        });
    }
}