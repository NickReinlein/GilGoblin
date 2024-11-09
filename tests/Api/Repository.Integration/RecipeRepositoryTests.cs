using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.Database.Integration;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Repository.Integration;

public class RecipeRepositoryTests : GilGoblinDatabaseFixture
{
    private IRecipeCache _recipeCache;
    private IItemRecipeCache _itemRecipeCache;
    private RecipeRepository _recipeRepo;

    [SetUp]
    public override async Task SetUp()
    {
        _recipeCache = Substitute.For<IRecipeCache>();
        _itemRecipeCache = Substitute.For<IItemRecipeCache>();

        await base.SetUp();
        _recipeRepo = new RecipeRepository(_serviceProvider, _recipeCache, _itemRecipeCache);
    }

    #region GetAll

    [Test]
    public async Task GivenWeGetAll_ThenTheRepositoryReturnsAllEntries()
    {
        await using var ctx = GetDbContext();
        var recipes = await ctx.Recipe.ToListAsync();

        var result = _recipeRepo.GetAll().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(recipes.Count));
            Assert.That(recipes.All(r => result.Any(s => s.Id == r.Id)));
        });
    }

    #endregion GetAll

    #region Get

    [TestCaseSource(nameof(ValidRecipeIds))]
    public void GivenWeGet_WhenTheIdIsValid_ThenTheRepositoryReturnsTheCorrectEntry(int id)
    {
        var result = _recipeRepo.Get(id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(id));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(99999)]
    public void GivenWeGet_WhenIdIsInvalid_ThenTheRepositoryReturnsNull(int id)
    {
        var result = _recipeRepo.Get(id);

        Assert.That(result, Is.Null);
    }

    [TestCaseSource(nameof(ValidRecipeIds))]
    public void GivenWeGet_WhenTheIdIsValidAndUncached_ThenWeCacheTheEntry(int recipeId)
    {
        _recipeCache.Get(recipeId).Returns((RecipePoco)null!);

        var result = _recipeRepo.Get(recipeId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(recipeId));
        _recipeCache.Received(1).Get(recipeId);
        _recipeCache.Received(1).Add(recipeId, Arg.Is<RecipePoco>(recipe => recipe.Id == recipeId));
    }

    [Test]
    public void GivenWeGet_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry()
    {
        const int recipeId = 44;
        var poco = new RecipePoco { Id = recipeId };
        _recipeCache.Get(recipeId).Returns(poco);

        var result = _recipeRepo.Get(recipeId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(poco.Id));
        _recipeCache.Received(1).Get(recipeId);
        _recipeCache.DidNotReceive().Add(recipeId, Arg.Any<RecipePoco>());
    }

    #endregion Get

    #region GetRecipesForItem

    [TestCaseSource(nameof(ValidItemsIds))]
    public async Task GivenWeGetRecipesForItem_WhenTheIdIsValidAndUncached_ThenWeReturnEntriesAndCacheThem(
        int targetItemId)
    {
        await using var ctx = GetDbContext();
        var expectedResultsCount = ctx.Recipe.Count(r => r.TargetItemId == targetItemId);

        var result = _recipeRepo.GetRecipesForItem(targetItemId);

        Assert.That(result, Has.Count.EqualTo(expectedResultsCount));
        _itemRecipeCache.Received(1).Get(targetItemId);
        _itemRecipeCache.Received(expectedResultsCount > 0 ? 1 : 0).Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    [TestCaseSource(nameof(ValidItemsIds))]
    public async Task GivenGetRecipesForItem_WhenTheIdIsValidAndCached_ThenWeReturnTheCachedEntry(int targetItemId)
    {
        await using var ctx = GetDbContext();
        var recipeCount = ctx.Recipe.Count(r => r.TargetItemId == targetItemId);

        var result = _recipeRepo.GetRecipesForItem(targetItemId);

        _itemRecipeCache.Received(1).Get(targetItemId);
        Assert.That(result, Has.Count.EqualTo(recipeCount));
        Assert.That(result.All(r => r.TargetItemId == targetItemId));
        // if (recipeCount > 0)
            // _itemRecipeCache.DidNotReceive().Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void GivenWeGetRecipesForItem_WhenTheIdIsInvalid_ThenTheRepositoryReturnsAnEmptyResult(int targetItemId)
    {
        var result = _recipeRepo.GetRecipesForItem(targetItemId);

        Assert.That(!result.Any());
        _itemRecipeCache.DidNotReceive().Get(targetItemId);
        _itemRecipeCache.DidNotReceive().Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    #endregion GetRecipesForItem

    #region GetRecipesForItemIds

    [Test]
    public void
        GivenWeGetRecipesForItems_WhenTheIdsAreValidAndUncached_ThenTheRepositoryReturnsTheEntriesAndCachesThem()
    {
        using var ctx = GetDbContext();
        var targetRecipes = ctx.Recipe.Where(r => ValidItemsIds.Contains(r.TargetItemId)).ToList();

        var result = _recipeRepo.GetRecipesForItemIds(ValidItemsIds);

        Assert.That(result, Has.Count.EqualTo(targetRecipes.Count));
        foreach (var recipe in targetRecipes)
        {
            _recipeCache.Received(1).Add(recipe.Id,
                Arg.Is<RecipePoco>(r =>
                    r.Id == recipe.Id &&
                    r.TargetItemId == recipe.TargetItemId));
            _itemRecipeCache.Received().Add(recipe.TargetItemId, Arg.Any<List<RecipePoco>>());
        }
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void GivenWeGetRecipesForItemIds_WhenTheIdIsInvalid_ThenTheRepositoryReturnsAnEmptyResultImmediately(
        int targetItemId
    )
    {
        var idList = new List<int> { targetItemId };

        var result = _recipeRepo.GetRecipesForItemIds(idList);

        Assert.That(!result.Any());
        _itemRecipeCache.DidNotReceive().Get(targetItemId);
        _itemRecipeCache.DidNotReceive().Add(targetItemId, Arg.Any<List<RecipePoco>>());
    }

    #endregion GetRecipesForItemIds

    #region GetMultiple

    [Test]
    public void GivenWeGetMultiple_WhenIdsAreValid_ThenTheCorrectEntriesAreReturned()
    {
        var result = _recipeRepo.GetMultiple(ValidRecipeIds);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidRecipeIds.Count));
            Assert.That(ValidRecipeIds.All(v => result.Any(p => p.Id == v)));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenSomeIdsAreValid_ThenTheValidEntriesAreReturned()
    {
        var mixedIds = ValidRecipeIds.Concat([8578]).ToList();

        var result = _recipeRepo.GetMultiple(mixedIds);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(ValidRecipeIds.Count));
            Assert.That(ValidRecipeIds.All(v => result.Any(p => p.Id == v)));
            Assert.That(result.All(r => r.Id != 8578));
        });
    }

    [Test]
    public void GivenWeGetMultiple_WhenIdsAreInvalid_ThenNoEntriesAreReturned()
    {
        var result = _recipeRepo.GetMultiple([654645646, 9953121]);

        Assert.That(!result.Any());
    }

    [Test]
    public void GivenWeGetMultiple_WhenIdsEmpty_ThenNoEntriesAreReturned()
    {
        var result = _recipeRepo.GetMultiple([]);

        Assert.That(!result.Any());
    }

    #endregion GetMultiple

    [Test]
    public async Task GivenWeFillCache_WhenEntriesExist_ThenWeFillTheCache()
    {
        await using var ctx = GetDbContext();
        var allRecipes = ctx.Recipe.ToList();

        await _recipeRepo.FillCache();

        allRecipes.ForEach(recipe =>
            _recipeCache.Received(1).Add(recipe.Id,
                Arg.Is<RecipePoco>(r =>
                    r.Id == recipe.Id && r.TargetItemId == recipe.TargetItemId &&
                    r.AmountIngredient0 == recipe.AmountIngredient0 &&
                    r.ItemIngredient0TargetId == recipe.ItemIngredient0TargetId)));
    }
}