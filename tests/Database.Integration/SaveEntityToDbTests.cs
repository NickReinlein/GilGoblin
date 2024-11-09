using System.Threading.Tasks;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

[TestFixture]
public abstract class SaveEntityToDbTests<T> : GilGoblinDatabaseFixture where T : class
{
    [Test]
    public virtual async Task GivenValidNewEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();

        await SaveEntityToDatabase(entity);

        await ValidateResultSavedToDatabase(entity);
    }

    [Test]
    public virtual async Task GivenValidExistingEntity_WhenSaving_ThenEntityIsSavedSuccessfully()
    {
        var entity = GetEntity();
        await SaveEntityToDatabase(entity);
        var modifiedSavedEntity = GetModifiedEntity(entity);

        await SaveEntityToDatabase(modifiedSavedEntity, true);

        await ValidateResultSavedToDatabase(modifiedSavedEntity);
    }

    protected virtual async Task SaveEntityToDatabase(T entity, bool update = false)
    {
        await using var ctx = GetDbContext();
        if (update)
            ctx.Update(entity);
        else
            await ctx.Set<T>().AddAsync(entity);

        var savedCount = await ctx.SaveChangesAsync();
        Assert.That(savedCount, Is.EqualTo(1));
    }

    protected abstract T GetEntity();
    protected abstract T GetModifiedEntity(T entity);
    protected abstract Task ValidateResultSavedToDatabase(T entity);
}