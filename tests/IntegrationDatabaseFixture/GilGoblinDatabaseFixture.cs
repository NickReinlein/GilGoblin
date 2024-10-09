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

namespace GilGoblin.Tests.IntegrationDatabaseFixture;

public class GilGoblinDatabaseFixture
{
    protected PostgreSqlContainer _postgresContainer;
    protected IConfigurationRoot _configuration;
    protected DbContextOptions<GilGoblinDbContext> _options;
    protected ServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;

    protected static readonly List<int> ValidWorldIds = [34, 99];
    protected static readonly List<int> ValidItemsIds = [134, 585, 654, 847];
    protected static readonly List<int> ValidRecipeIds = [111, 122];

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await StartContainerAsync();

        ConfigureServices();

        await EnsureDatabaseIsCreated();
    }

    [SetUp]
    public virtual async Task SetUp()
    {
        await DeleteAllEntriesAsync();
        await CreateAllEntriesAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _postgresContainer.DisposeAsync();
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        await DeleteAllEntriesAsync();
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
        var qualities = new[] { true, false };

        var allRecipes =
            (from id in ValidRecipeIds
                select new RecipePoco
                {
                    Id = id,
                    CanHq = id % 2 == 0,
                    CraftType = id % 2 == 0 ? 3 : 7,
                    CanQuickSynth = id % 2 == 0,
                    ResultQuantity = 1,
                    RecipeLevelTable = 123,
                    TargetItemId = ValidItemsIds.ElementAt(Random.Shared.Next(ValidItemsIds.Count)),
                    AmountIngredient0 = id + 2,
                    ItemIngredient0TargetId = id + 3
                })
            .ToList();
        await context.Recipe.AddRangeAsync(allRecipes);

        var itemList = ValidItemsIds.Select(i => new ItemPoco
            {
                Id = i,
                Name = $"Item {i}",
                Description = $"Item {i} description",
                IconId = i + 111,
                CanHq = i % 2 == 0,
                PriceLow = i * 3 + 11,
                PriceMid = i * 3 + 12,
                Level = i + 13,
                StackSize = 1
            })
            .ToList();
        await context.Item.AddRangeAsync(itemList);

        var validWorlds = ValidWorldIds
            .Select(w => new WorldPoco { Id = w, Name = $"World {w}" })
            .ToList();
        await context.World.AddRangeAsync(validWorlds);

        var updateTime = DateTimeOffset.UtcNow.AddHours(-1);
        var allPrices =
            (from itemId in ValidItemsIds
                from worldId in ValidWorldIds
                from quality in qualities
                select new PricePoco(itemId, worldId, quality, updateTime))
            .ToList();

        await context.Price.AddRangeAsync(allPrices);

        await context.SaveChangesAsync();
    }

    protected GilGoblinDbContext GetDbContext() => new(_options, _configuration);

    protected string GetConnectionString() => _postgresContainer.GetConnectionString() ??
                                              throw new InvalidOperationException("Connection string not found.");
}