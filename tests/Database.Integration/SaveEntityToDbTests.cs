using System.Threading.Tasks;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

[TestFixture]
public abstract class SaveEntityToDbTests<T> : GilGoblinDatabaseFixture
    where T : class
{
    [Test]
    public async Task GivenValidNewEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();

        await SavePocoToDatabase(entity);

        await ValidateResultSavedToDatabaseAsync(entity);
    }

    [Test]
    public async Task GivenValidExistingEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();
        await SavePocoToDatabase(entity);
        var modifiedEntity = GetModifiedEntity(entity);

        await SavePocoToDatabase(modifiedEntity, true);

        await ValidateResultSavedToDatabaseAsync(modifiedEntity);
    }

    private async Task SavePocoToDatabase(T entity, bool update = false)
    {
        await using var ctx = GetNewDbContext();
        if (update)
            ctx.Set<T>().Update(entity);
        else
            await ctx.Set<T>().AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }

    protected abstract T GetEntity();
    protected abstract T GetModifiedEntity(T entity);
    protected abstract Task ValidateResultSavedToDatabaseAsync(T entity);
}