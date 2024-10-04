using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class ItemSavingTests : SaveEntityToDbTests<ItemPoco>
{
    protected override async Task ValidateResultSavedToDatabaseAsync(ItemPoco entity)
    {
        await using var ctx = GetNewDbContext();
        var result = await ctx.Item.FirstOrDefaultAsync(
            x => x.Name == entity.Name &&
                 x.Description == entity.Description &&
                 x.IconId == entity.IconId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(entity.Name));
            Assert.That(result.IconId, Is.EqualTo(entity.IconId));
            Assert.That(result.CanHq, Is.EqualTo(entity.CanHq));
            Assert.That(result.Description, Is.EqualTo(entity.Description));
            Assert.That(result.PriceLow, Is.EqualTo(entity.PriceLow));
            Assert.That(result.PriceMid, Is.EqualTo(entity.PriceMid));
            Assert.That(result.Level, Is.EqualTo(entity.Level));
            Assert.That(result.StackSize, Is.EqualTo(entity.StackSize));
        });
    }

    protected override ItemPoco GetEntity() =>
        new()
        {
            Name = "item 100",
            Description = "item 100 description",
            IconId = 11,
            Level = 12,
            StackSize = 1,
            PriceLow = 111,
            PriceMid = 222,
            CanHq = true
        };

    protected override ItemPoco GetModifiedEntity(ItemPoco entity)
    {
        entity.Description += "new description";
        entity.Name += "new name";
        entity.PriceLow += 11;
        entity.PriceMid += 11;
        entity.Level += 11;
        entity.StackSize += 11;
        entity.IconId = 11;
        return entity;
    }
}