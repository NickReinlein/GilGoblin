using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Controllers;
using GilGoblin.Api.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Api.Controllers;

public class CraftControllerTests
{
    private CraftController _controller = null!;
    private ICraftRepository<CraftSummaryPoco> _repo;

    private const int world = 34;
    private const int recipe = 221;

    [SetUp]
    public void SetUp()
    
    {
        _repo = Substitute.For<ICraftRepository<CraftSummaryPoco>>();

        _controller = new CraftController(
            _repo,
            NullLoggerFactory.Instance.CreateLogger<CraftController>()
        );
    }

    [Test]
    public void WhenWeSetup_ThenControllerIsSuccessfullyCreated()
    {
        Assert.That(_controller, Is.Not.Null);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCrafts_ThenTheRepositoryIsCalled()
    {
        _repo.GetBestAsync(world).Returns(new List<CraftSummaryPoco>());

        _ = await _controller.GetBestAsync(world);

        await _repo.Received(1).GetBestAsync(world);
    }

    [Test]
    public async Task WhenReceivingARequestGetBestCrafts_ThenAnEnumerableIsReturned()
    {
        _repo.GetBestAsync(world).Returns(new List<CraftSummaryPoco>());

        var result = await _controller.GetBestAsync(world);

        Assert.That(result, Is.Not.Null.Or.Empty);
    }    
    
    [Test]
    public async Task WhenReceivingARequestGetCrafts_ThenTheRepositoryIsCalled()
    {
        _repo.GetAsync(world, recipe).Returns(new CraftSummaryPoco());

        _ = await _controller.GetAsync(world, recipe);

        await _repo.Received(1).GetAsync(world, recipe);
    }

    [Test]
    public async Task WhenReceivingARequestGetCrafts_ThenAnEnumerableIsReturned()
    {
        _repo.GetAsync(world, recipe).Returns(new CraftSummaryPoco());

        var result = await _controller.GetAsync(world, recipe);

        Assert.That(result, Is.Not.Null);
    }
}