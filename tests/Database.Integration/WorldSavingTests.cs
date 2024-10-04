using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class WorldSavingTests : SaveEntityToDbTests<WorldPoco>
{
    protected override WorldPoco GetEntity() => new() { Name = "World" };

    protected override WorldPoco GetModifiedEntity(WorldPoco entity) =>
        new() { Id = entity.Id, Name = $"{entity.Name} modified" };

    protected override async Task ValidateResultSavedToDatabaseAsync(WorldPoco entity)
    {
        await using var ctx = GetNewDbContext();

        var result = await ctx.World.FirstOrDefaultAsync(x => x.Id == entity.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(entity.Id));
            Assert.That(result.Name, Is.EqualTo(entity.Name));
        });
    }
}