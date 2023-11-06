using System.Threading;
using System.Threading.Tasks;
using GilGoblin.DataUpdater;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class PriceUpdaterTests
{
    private PriceUpdater _priceUpdater;
    private IServiceScopeFactory _scopeFactory;
    private ILogger<PriceUpdater> _logger;

    [SetUp]
    public void SetUp()
    {
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<PriceUpdater>>();
        _priceUpdater = new PriceUpdater(_scopeFactory, _logger);
    }

    [Test]
    public async Task GivenAFetchAsync_WhenIdsAreReturnedToUpdate_ThenWeFetchUpdatesForThoseIds()
    {
        await _priceUpdater.FetchAsync(CancellationToken.None, 1);
        
        // Assert.That(result, Is.Not.Null);
    }
}