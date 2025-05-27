using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class GilGoblinDatabaseFixture
{
    protected IContainer _postgresContainer = null!;
    protected IConfigurationRoot _configuration = null!;
    protected IServiceProvider _serviceProvider = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    private uint _mappedPort;

    protected static readonly List<int> ValidWorldIds = [34, 99];
    protected static readonly List<int> ValidItemsIds = [134, 585, 654, 847];
    protected static readonly List<int> ValidRecipeIds = [111, 122];

    private const string Username = "gilgoblin";
    private const string DatabaseName = "gilgoblin_db";
    private const string Password = "gilgoblin_password";

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        _postgresContainer = new ContainerBuilder()
            .WithImage("nickreinlein/gilgoblin-database:latest")
            .WithPortBinding(5432, true)
            .WithEnvironment("POSTGRES_DB", DatabaseName)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        await _postgresContainer.StartAsync();
        _mappedPort = _postgresContainer.GetMappedPublicPort(5432);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GilGoblinDbContext"] = GetConnectionString()
            })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(_configuration)
            .AddDbContext<GilGoblinDbContext>(opts =>
                opts.UseNpgsql(GetConnectionString())
                    .UseSnakeCaseNamingConvention()
                    .EnableDetailedErrors(false)
                    .EnableSensitiveDataLogging(false)
                    .LogTo(Console.WriteLine, LogLevel.Information)
            );

        _serviceProvider = services.BuildServiceProvider();
        _scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        await EnsureDatabaseIsCreatedAsync();
        await DeleteAllEntriesAsync();
        await CreateAllEntriesAsync();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        await DeleteAllEntriesAsync();
        await CreateAllEntriesAsync();
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
    }

    private async Task EnsureDatabaseIsCreatedAsync()
    {
        var cs = GetConnectionString();
        Console.WriteLine($"[DEBUG] EF using connection string: {cs}");

        await using var ctx = GetDbContext();
        const int attempts = 10;
        for (var i = 1; i <= attempts; i++)
        {
            try
            {
                Console.WriteLine($"[DEBUG] EF try {i}: OpenConnectionAsync()");
                await ctx.Database.OpenConnectionAsync();
                await ctx.Database.EnsureCreatedAsync();
                Console.WriteLine("[DEBUG] EF connection opened successfully");
                await ctx.Database.CloseConnectionAsync();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] EF attempt {i} failed: {ex.GetType().Name} – {ex.Message}");
                if (i == attempts)
                    throw new Exception($"EF couldn't connect after {attempts} tries: {ex.Message}", ex);
                await Task.Delay(500);
            }
        }
    }

    private async Task DeleteAllEntriesAsync()
    {
        Console.WriteLine("[DEBUG] Deleting all entries from the database");
        var tableOrder = new[]
        {
            "recipe_profit", 
            "recipe_cost", 
            "world_upload_times",
            "price", 
            "recent_purchase", 
            "average_sale_price", 
            "min_listing",
            "daily_sale_velocity", 
            "price_data", 
            "recipe", 
            "item", 
            "world" 
        };

        await using var ctx = GetDbContext();
        foreach (var table in tableOrder)
        {
            Console.WriteLine($"[DEBUG] Deleting entries from table: {table}");
            var sql = $"DELETE FROM \"{table}\";";
            await ctx.Database.ExecuteSqlRawAsync(sql);
        }
        Console.WriteLine("[DEBUG] All entries deleted successfully");
    }

    protected async Task CreateAllEntriesAsync()
    {
        await using var ctx = GetDbContext();
        var qualities = new[] { true, false };
        var items = ValidItemsIds.Select(i =>
                new ItemPoco
                {
                    Id = i,
                    Name = $"Item {i}",
                    Description = $"Functions as an item with ID {i}",
                    IconId = i + 111,
                    StackSize = 1,
                    PriceLow = i * 3 + 11,
                    PriceMid = i * 3 + 12,
                    Level = i + 13,
                    CanHq = i % 2 == 0
                })
            .ToList();
        await ctx.Item.AddRangeAsync(items);

        var recipes = ValidRecipeIds
            .Select(id => new RecipePoco
            {
                Id = id,
                CanHq = id % 2 == 0,
                CraftType = id % 2 == 0 ? 3 : 7,
                CanQuickSynth = id % 2 == 0,
                ResultQuantity = 1,
                RecipeLevelTable = 123,
                TargetItemId = ValidItemsIds[new Random().Next(ValidItemsIds.Count)],
                AmountIngredient0 = id + 2,
                ItemIngredient0TargetId = id + 3
            })
            .ToList();
        await ctx.Recipe.AddRangeAsync(recipes);

        var worlds = ValidWorldIds
            .Select(w => new WorldPoco { Id = w, Name = $"World {w}" })
            .ToList();
        await ctx.World.AddRangeAsync(worlds);

        var prices = (from it in ValidItemsIds
            from w in ValidWorldIds
            from q in qualities
            select new PricePoco(it, w, q)).ToList();
        await ctx.Price.AddRangeAsync(prices);

        var recipeCosts =
            (from recipeId in ValidRecipeIds
                from worldId in ValidWorldIds
                from quality in qualities
                select new RecipeCostPoco(
                    recipeId,
                    worldId,
                    quality,
                    Random.Shared.Next(120, 800),
                    DateTimeOffset.UtcNow))
            .ToList();
        await ctx.RecipeCost.AddRangeAsync(recipeCosts);

        var recipeProfits =
            (from recipeId in ValidRecipeIds
                from worldId in ValidWorldIds
                from quality in qualities
                select new RecipeProfitPoco(
                    recipeId,
                    worldId,
                    quality,
                    Random.Shared.Next(120, 800),
                    DateTimeOffset.UtcNow))
            .ToList();
        await ctx.RecipeProfit.AddRangeAsync(recipeProfits);

        await ctx.SaveChangesAsync();
    }
    protected string GetConnectionString()
    {
        // Use DB_PORT from environment if set (for CI), otherwise use _mappedPort (for local Testcontainers)
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var portToUse = !string.IsNullOrEmpty(port) ? port : _mappedPort.ToString();
        return $"Host=localhost;Port={portToUse};Database={DatabaseName};Username={Username};Password={Password}";
    }

    protected GilGoblinDbContext GetDbContext()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
    }
}