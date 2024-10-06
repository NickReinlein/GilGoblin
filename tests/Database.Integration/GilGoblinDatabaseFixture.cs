using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace GilGoblin.Tests.Database.Integration;

public class GilGoblinDatabaseFixture
{
    protected PostgreSqlContainer _postgresContainer;
    protected IConfigurationRoot _configuration;
    protected DbContextOptions<GilGoblinDbContext> _options;
    protected ServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;

    protected static readonly List<int> ValidWorldIds = [34, 99];
    protected static readonly List<int> ValidItemsIds = [134, 584, 654, 842];
    protected static readonly List<int> ValidRecipeIds = [111, 122];

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await StartContainerAsync();

        ConfigureServices();

        await EnsureDatabaseIsCreated();
    }

    private void ConfigureServices()
    {
        var connectionString = GetConnectionString();
        var configKvps = new Dictionary<string, string?>
        {
            { "ConnectionStrings:GilGoblinDbContext", connectionString }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configKvps)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(_configuration)
            .AddDbContext<GilGoblinDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            })
            .BuildServiceProvider();
        _options = _serviceProvider
            .GetRequiredService<DbContextOptions<GilGoblinDbContext>>();
        _serviceScope = Substitute.For<IServiceScope>();
        _serviceScope.ServiceProvider.Returns(_serviceProvider);
    }

    private async Task EnsureDatabaseIsCreated()
    {
        await using var ctx = GetDbContext();
        if (!await ctx.Database.EnsureCreatedAsync() ||
            !await ctx.Database.CanConnectAsync())
            throw new Exception("Failed to connect to database");
    }

    private async Task StartContainerAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("goblin_db")
            .WithUsername("goblin")
            .WithPassword("goblin_pw")
            .WithPortBinding(0, 5432)
            .WithCleanUp(true)
            .Build();
        await _postgresContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _postgresContainer.DisposeAsync();
    }

    [SetUp]
    public virtual async Task SetUp()
    {
        await DeleteAllEntriesAsync();
        await CreateAllEntriesAsync();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        await DeleteAllEntriesAsync();
    }

    protected async Task DeleteAllEntriesAsync()
    {
        await using var context = new GilGoblinDbContext(_options, _configuration);
        context.RecentPurchase.RemoveRange(context.RecentPurchase);
        context.AverageSalePrice.RemoveRange(context.AverageSalePrice);
        context.MinListing.RemoveRange(context.MinListing);
        context.DailySaleVelocity.RemoveRange(context.DailySaleVelocity);
        context.World.RemoveRange(context.World);
        context.Item.RemoveRange(context.Item);
        context.Recipe.RemoveRange(context.Recipe);
        context.RecipeCost.RemoveRange(context.RecipeCost);
        context.RecipeProfit.RemoveRange(context.RecipeProfit);
        context.Price.RemoveRange(context.Price);
        context.PriceData.RemoveRange(context.PriceData);
        context.WorldUploadTime.RemoveRange(context.WorldUploadTime);
        await context.SaveChangesAsync();
    }

    protected async Task CreateAllEntriesAsync()
    {
        await using var context = GetDbContext();

        var recipeList = new RecipePoco[]
        {
            new()
            {
                Id = ValidRecipeIds[0],
                TargetItemId = ValidItemsIds[0],
                ItemIngredient0TargetId = ValidItemsIds[1],
                AmountIngredient0 = 2,
                CanHq = true,
                CraftType = 3,
                CanQuickSynth = true,
                ResultQuantity = 1,
                RecipeLevelTable = 123
            },
            new()
            {
                Id = ValidRecipeIds[1],
                TargetItemId = ValidItemsIds[2],
                ItemIngredient0TargetId = ValidItemsIds[3],
                AmountIngredient0 = 3,
                CanHq = true,
                CraftType = 7,
                CanQuickSynth = true,
                ResultQuantity = 2,
                RecipeLevelTable = 123
            }
        }.ToList();
        await context.Recipe.AddRangeAsync(recipeList);

        var itemList = ValidItemsIds.Select(i => new ItemPoco
        {
            Name = $"Item {i}",
            Description = $"Item {i} description",
            IconId = i + 111,
            CanHq = i % 2 == 0,
            PriceLow = i * 3 + 11,
            PriceMid = i * 3 + 12,
            Level = i + 13,
            StackSize = 1
        }).ToList();
        await context.Item.AddRangeAsync(itemList);

        var validWorlds = ValidWorldIds
            .Select(w => new WorldPoco { Id = w, Name = $"World {w}" })
            .ToList();
        await context.World.AddRangeAsync(validWorlds);

        var qualities = new[] { true, false };
        var updateTime = DateTimeOffset.UtcNow.AddHours(-1);
        var allPrices =
            (from itemId in ValidItemsIds
                from worldId in ValidWorldIds
                from quality in qualities
                select new PricePoco(itemId, worldId, quality, updateTime)).ToList();

        await context.Price.AddRangeAsync(allPrices);

        await context.SaveChangesAsync();
    }

    protected GilGoblinDbContext GetDbContext() => new(_options, _configuration);

    protected string GetConnectionString() => _postgresContainer.GetConnectionString() ??
                                              throw new InvalidOperationException("Connection string not found.");
}