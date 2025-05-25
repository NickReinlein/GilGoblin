using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.Database.Integration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class RecipeCostRepositoryTests : GilGoblinDatabaseFixture
{
    private ICalculatedMetricCache<RecipeCostPoco> _cache;
    private RecipeCostRepository _repo;

    [SetUp]
    public void SetUp()
    {
        _cache = Substitute.For<ICalculatedMetricCache<RecipeCostPoco>>();
        _repo = new RecipeCostRepository(_serviceProvider, _cache);
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenAGetAll_WhenEntriesExistForThatWorld_ThenTheRepositoryReturnsAllEntriesForThatWorld(
        int worldId)
    {
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeCost.Where(c => c.WorldId == worldId).ToList();

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
    public async Task GivenAGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(TripleKey data)
    {
        var worldId = data.WorldId;
        var itemId = data.Id;
        var isHq = data.IsHq;
        await using var context = GetDbContext();
        var expectedRecipes = context.RecipeCost
            .FirstOrDefault(c =>
                c.WorldId == worldId &&
                c.RecipeId == itemId &&
                c.IsHq == isHq);

        var result = await _repo.GetAsync(data.WorldId, data.Id, data.IsHq);

        Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!, Is.EqualTo(expectedRecipes));
            }
        );
    }

    [TestCaseSource(nameof(GetValidRecipeKeys))]
    public async Task GivenAGet_WhenTheIdIsValidButTheWorldIdIsNot_ThenNullIsReturned(TripleKey data)
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