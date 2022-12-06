using GilGoblin.Crafting;
using GilGoblin.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DI;

public class GilGoblinService : IHostedService
{
    private readonly IRecipeGateway _recipeGateway;
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly IRecipeGrocer _recipeGrocer;
    private readonly ILogger<GilGoblinService> _log;

    public GilGoblinService(IRecipeGateway recipeGateway, IMarketDataGateway marketDataGateway, IRecipeGrocer recipeGrocer, ILogger<GilGoblinService> log)
    {
        _log = log;
        _recipeGateway = recipeGateway;
        _marketDataGateway = marketDataGateway;
        _recipeGrocer = recipeGrocer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("Starting Gilgoblin Service.");
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(new TimeSpan(0, 0, 2)); // 2 second delay'
                var getItemTest = _recipeGateway.GetRecipe(12);
                if (getItemTest is null) throw new Exception("Failed here");
            }
        });

        _log.LogWarning("Gilgoblin Service ended.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("Gilgoblin Service was stopped.");
        return Task.CompletedTask;
    }
}