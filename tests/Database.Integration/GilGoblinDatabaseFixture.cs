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

    protected static readonly List<int> ValidWorldIds = [34, 99];
    protected static readonly List<int> ValidItemsIds = [134, 585, 654, 847];
    protected static readonly List<int> ValidRecipeIds = [111, 122];

    private const string Username = "gilgoblin";
    private const string DatabaseName = "gilgoblin_db";
    private const string Password = "gilgoblin_password";
    private const ushort HostPort = 52348;

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        _postgresContainer = new ContainerBuilder()
            .WithImage("nickreinlein/gilgoblin-database:latest")
            .WithPortBinding(HostPort, 5432)
            .WithEnvironment("POSTGRES_DB", DatabaseName)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(5432))
            .Build();

        await _postgresContainer.StartAsync();

        var conn = $"Host=localhost;Port={HostPort};Database={DatabaseName};Username={Username};Password={Password}";
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:GilGoblinDbContext"] = conn })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(_configuration)
            .AddDbContext<GilGoblinDbContext>(opts =>
                opts.UseNpgsql(conn)
                    .EnableDetailedErrors(false)
                    .EnableSensitiveDataLogging(false)
                    .LogTo(Console.WriteLine, LogLevel.Warning)
            );

        _serviceProvider = services.BuildServiceProvider();

        await EnsureDatabaseIsCreated();
        await CreateAllEntriesAsync();
    }

    [TearDown]
    public virtual Task TearDown() => DeleteAllEntriesAsync();

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
    }

    protected GilGoblinDbContext GetDbContext() =>
        _serviceProvider.GetRequiredService<GilGoblinDbContext>();

    private async Task EnsureDatabaseIsCreated()
    {
        await using var ctx = GetDbContext();
        const int max = 5;
        for (var i = 0; i < max; i++)
        {
            try
            {
                await ctx.Database.EnsureCreatedAsync();
                if (await ctx.Database.CanConnectAsync())
                    return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB connect attempt {i + 1} failed: {ex.Message}");
            }

            await Task.Delay(1_000);
        }

        throw new Exception($"Could not connect to Postgres after {max} attempts");
    }

    private async Task DeleteAllEntriesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        await using var ctx = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        ctx.RecentPurchase.RemoveRange(ctx.RecentPurchase);
        ctx.AverageSalePrice.RemoveRange(ctx.AverageSalePrice);
        ctx.MinListing.RemoveRange(ctx.MinListing);
        ctx.DailySaleVelocity.RemoveRange(ctx.DailySaleVelocity);
        ctx.World.RemoveRange(ctx.World);
        ctx.Item.RemoveRange(ctx.Item);
        ctx.Recipe.RemoveRange(ctx.Recipe);
        ctx.RecipeCost.RemoveRange(ctx.RecipeCost);
        ctx.RecipeProfit.RemoveRange(ctx.RecipeProfit);
        ctx.Price.RemoveRange(ctx.Price);
        ctx.PriceData.RemoveRange(ctx.PriceData);
        ctx.WorldUploadTime.RemoveRange(ctx.WorldUploadTime);
        await ctx.SaveChangesAsync();
    }

    protected async Task CreateAllEntriesAsync()
    {
        await using var ctx = GetDbContext();
        var qualities = new[] { true, false };

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

        var items = ValidItemsIds
            .Select(i => new ItemPoco
            {
                Id = i,
                Name = $"Item {i}",
                IconId = i + 111,
                StackSize = 1,
                PriceLow = i * 3 + 11,
                PriceMid = i * 3 + 12,
                Level = i + 13,
                CanHq = i % 2 == 0
            })
            .ToList();
        await ctx.Item.AddRangeAsync(items);

        var worlds = ValidWorldIds
            .Select(w => new WorldPoco { Id = w, Name = $"World {w}" })
            .ToList();
        await ctx.World.AddRangeAsync(worlds);

        var prices = (from it in ValidItemsIds
            from w in ValidWorldIds
            from q in qualities
            select new PricePoco(it, w, q)).ToList();
        await ctx.Price.AddRangeAsync(prices);

        await ctx.SaveChangesAsync();
    }

    protected string GetConnectionString()
    {
        return $"Host=localhost;Port={HostPort};Database={DatabaseName};Username={Username};Password={Password}";
    }
}