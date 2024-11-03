using System.Threading.Tasks;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

[TestFixture]
public abstract class SaveEntityToDbTests<T> : GilGoblinDatabaseFixture
    where T : class
{
    [Test]
    public virtual async Task GivenValidNewEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();

        await SavePocoToDatabase(entity);

        await ValidateResultSavedToDatabaseAsync(entity);
    }

    [Test]
    public virtual async Task GivenValidExistingEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();
        entity = await SavePocoToDatabase(entity);
        var modifiedSavedEntity = GetModifiedEntity(entity);

        await SavePocoToDatabase(modifiedSavedEntity, true);

        await ValidateResultSavedToDatabaseAsync(modifiedSavedEntity);
    }

    protected virtual async Task<T> SavePocoToDatabase(T entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        if (update)
            ctx.Update(entity);
        else
            await ctx.AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
        return entity;
    }

    protected abstract T GetEntity();
    protected abstract T GetModifiedEntity(T entity);
    protected abstract Task ValidateResultSavedToDatabaseAsync(T entity);
}