using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class WorldSavingTests : SaveEntityToDbTests<WorldPoco>
{
    protected override WorldPoco GetEntity() => new() { Id = 11, Name = "World" };

    protected override WorldPoco GetModifiedEntity(WorldPoco entity) =>
        entity with { Name = $"{entity.Name} modified" };

    protected override async Task ValidateResultSavedToDatabase(WorldPoco entity)
    {
        await using var ctx = GetDbContext();

        var result = await ctx.World
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entity.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(entity.Id));
            Assert.That(result.Name, Is.EqualTo(entity.Name));
        });
    }
}