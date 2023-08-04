using GilGoblin.Database;
using GilGoblin.Repository;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeGatewayTests
{
    private RecipeGateway _recipeGateway;
    private IRecipeRepository _recipes;

    [SetUp]
    public void SetUp()
    {
        _recipes = Substitute.For<IRecipeRepository>();

        _recipeGateway = new RecipeGateway(_recipes);
    }

    [Test]
    public async Task GivenAGet_ThenTheRepositoryGetIsCalled()
    {
        await _recipeGateway.Get(1);

        await _recipes.Received(1).Get(1);
    }

    [Test]
    public async Task GivenAGetRecipesForItem_ThenTheRepositoryGetRecipesForItemIsCalled()
    {
        await _recipeGateway.GetRecipesForItem(1);

        await _recipes.Received(1).GetRecipesForItem(1);
    }

    [Test]
    public async Task GivenAGetMultiple_ThenTheRepositoryGetMultipleIsCalled()
    {
        var multiple = Enumerable.Range(1, 10);
        await _recipeGateway.GetMultiple(multiple);

        await _recipes.Received(1).GetMultiple(multiple);
    }

    [Test]
    public async Task GivenAGetGetAll_ThenTheRepositoryGetAllIsCalled()
    {
        await _recipeGateway.GetAll();

        await _recipes.Received(1).GetAll();
    }
}
