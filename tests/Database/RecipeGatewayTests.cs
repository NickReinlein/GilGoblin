using System.Data.Entity;
using GilGoblin.Database;
using GilGoblin.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RecipeGatewayTests
{
    private RecipeGateway _recipeGateway;
    private IContextFetcher _contextFetcher;
    private ILogger<RecipeGateway> _logger;

    [SetUp]
    public void SetUp()
    {
        _contextFetcher = Substitute.For<IContextFetcher>();
        _logger = Substitute.For<ILogger<RecipeGateway>>();

        _recipeGateway = new RecipeGateway(_contextFetcher, _logger);
    }

    [Test]
    public async Task GivenACallToGet_WhenTheIDIsValid_ThenWeReturnARecipe()
    {
        // _contextFetcher.GetContextAsync().Returns(null);
        var result = await _recipeGateway.Get(1);

        Assert.That(result, Is.Not.Null);
    }
}
