using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using GilGoblin.Tests.Database.Integration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class RecipeProfitRepositoryTests : GilGoblinDatabaseFixture
{
    private ICalculatedMetricCache<RecipeProfitPoco> _cache;
    private RecipeProfitRepository _repo;

    [SetUp]
    public override async Task SetUp()
    {
        _cache = Substitute.For<ICalculatedMetricCache<RecipeProfitPoco>>();
        await base.SetUp();

        _repo = new RecipeProfitRepository(_serviceProvider, _cache);
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenAGetAll_WhenEntriesExistForThatWorld_ThenTheRepositoryReturnsAllEntriesForThatWorld(
        int worldId)
    {
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeProfit.Where(c => c.WorldId == worldId).ToList();

        var result = await _repo.GetAllAsync(worldId);

        Assert.That(result, Has.Count.GreaterThan(1));
        Assert.That(result, Is.EquivalentTo(expectedRecipes));
    }

    [Test]
    public async Task GivenAGetAll_WhenTheWorldIdDoesNotExist_ThenAnEmptyResponseIsReturned()
    {
        var result = await _repo.GetAllAsync(999);

        Assert.That(!result.Any());
    }

    [TestCaseSource(nameof(GetValidRecipeKeys))]
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(TripleKey key)
    {
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeProfit
            .FirstOrDefault(c =>
                c.RecipeId == key.Id &&
                c.WorldId == key.WorldId &&
                c.IsHq == key.IsHq);

        var result = await _repo.GetAsync(key.WorldId, key.Id, key.IsHq);

        Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!, Is.EqualTo(expectedRecipes));
            }
        );
    }

    [TestCaseSource(nameof(GetValidRecipeKeys))]
    public async Task GivenAGet_WhenTheIdIsValidButTheWorldIdIsNot_ThenNullIsReturned(
        TripleKey data)
    {
        var result = await _repo.GetAsync(9898, data.Id, data.IsHq);

        Assert.That(result, Is.Null);
    }

    private static IEnumerable<TripleKey> GetValidRecipeKeys()
    {
        return from recipeId in ValidRecipeIds
            from worldId in ValidWorldIds
            from hq in new[] { true, false }
            select new TripleKey(worldId, recipeId, hq);
    }
}