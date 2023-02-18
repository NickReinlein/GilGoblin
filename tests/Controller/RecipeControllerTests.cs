using GilGoblin.Controller;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controller;

public class RecipeControllerTests
{
    private RecipeController _controller;
    private readonly IRecipeRepository _repo = Substitute.For<IRecipeRepository>();
    private static readonly int _recipeID = 108;
    private static readonly int _worldId = 34;

    [SetUp]
    public void SetUp()
    {
        _controller = new RecipeController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<RecipeController>()
        );
        Assert.That(_controller, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _repo.ClearReceivedCalls();
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfRecipesIsReturned()
    {
        _repo.GetAll().Returns(new List<RecipePoco>());

        var result = _controller.GetAll();

        Assert.That(result is not null);
    }

    [Test]
    public void GivenAController_WhenWeReceiveAGetRequest_ThenARecipeIsReturned()
    {
        _repo.Get(_recipeID).Returns(new RecipePoco());

        var result = _controller.Get(_recipeID);

        Assert.That(result is not null);
    }
}
