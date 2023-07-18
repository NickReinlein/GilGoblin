using GilGoblin.Controllers;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Controllers;

public class RecipeControllerTests
{
    private RecipeController _controller;
    private IRecipeRepository _repo;

    [SetUp]
    public void SetUp()
    {
        _repo = Substitute.For<IRecipeRepository>();
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
    public async Task GivenAController_WhenWeReceiveAGetAllRequest_ThenAListOfRecipesIsReturned()
    {
        var poco1 = CreatePoco();
        var poco2 = CreatePoco();
        poco2.ID = poco1.ID + 100;
        _repo.GetAll().Returns(new List<RecipePoco>() { poco1, poco2 });

        var result = await _controller.GetAll();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Count(i => i.ID == poco1.ID), Is.EqualTo(1));
        Assert.That(result.Count(i => i.ID == poco2.ID), Is.EqualTo(1));
    }

    [Test]
    public async Task GivenAController_WhenWeReceiveAGetRequest_ThenARecipeIsReturned()
    {
        var poco1 = CreatePoco();
        _repo.Get(poco1.ID).Returns(poco1);

        var result = await _controller.Get(poco1.ID);

        Assert.That(result, Is.EqualTo(poco1));
    }

    [Test]
    public async Task GivenAController_WhenWeReceiveAGetRequestForANonExistentRecipe_ThenNullIsReturned()
    {
        var result = await _controller.Get(42);

        Assert.That(result, Is.Null);
    }

    private static RecipePoco CreatePoco() =>
        new()
        {
            ID = 200,
            TargetItemID = 100,
            ResultQuantity = 1,
            CanHq = true,
            CanQuickSynth = true
        };
}
