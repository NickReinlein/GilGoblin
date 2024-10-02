using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace GilGoblin.Tests.Database.Integration;

public class PriceDataPointTests
{
    private PostgreSqlContainer? _postgresContainer;
    private IConfigurationRoot _configuration;
    private DbContextOptions<GilGoblinDbContext> _options;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
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

            Console.WriteLine($"PostgreSQL started on: {_postgresContainer.GetConnectionString()}");

            var configKvps = new Dictionary<string, string?>
            {
                { "ConnectionStrings:GilGoblinDbContext", GetConnectionString() }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configKvps)
                .Build();

            _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(GetConnectionString())
                .Options;

            await using var ctx = GetNewDbContext();
            var created = await ctx.Database.EnsureCreatedAsync();
            if (!created)
                throw new Exception("Failed to connect to database");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Test]
    public async Task GivenAverageSalePricePoco_IsValid_WhenSaving_ThenObjectIsSavedSuccessfully()
    {
        var averageSalePrice = new AverageSalePricePoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.AverageSalePrice.AddAsync(averageSalePrice);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.AverageSalePrice.FirstOrDefaultAsync(x => x.ItemId == averageSalePrice.ItemId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(averageSalePrice.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(averageSalePrice.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(averageSalePrice.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(averageSalePrice.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(averageSalePrice.RegionDataPointId));
        });
    }
    
    [Test]
    public async Task GivenRecentPurchasePocoPoco_IsValid_WhenSaving_ThenObjectIsSavedSuccessfully()
    {
        var recentPurchasePoco = new RecentPurchasePoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.RecentPurchase.AddAsync(recentPurchasePoco);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.RecentPurchase.FirstOrDefaultAsync(x => x.ItemId == recentPurchasePoco.ItemId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(recentPurchasePoco.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(recentPurchasePoco.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(recentPurchasePoco.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(recentPurchasePoco.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(recentPurchasePoco.RegionDataPointId));
        });
    }
    
    [Test]
    public async Task GivenMinListingPoco_IsValid_WhenSaving_ThenObjectIsSavedSuccessfully()
    {
        var minListing = new MinListingPoco(100, true, 100, 200, 300);

        await using var ctx = GetNewDbContext();
        await ctx.MinListing.AddAsync(minListing);
        var savedCount = await ctx.SaveChangesAsync();

        Assert.That(savedCount, Is.EqualTo(1));
        var result = await ctx.MinListing.FirstOrDefaultAsync(x => x.ItemId == minListing.ItemId);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ItemId, Is.EqualTo(minListing.ItemId));
            Assert.That(result.IsHq, Is.EqualTo(minListing.IsHq));
            Assert.That(result.WorldDataPointId, Is.EqualTo(minListing.WorldDataPointId));
            Assert.That(result.DcDataPointId, Is.EqualTo(minListing.DcDataPointId));
            Assert.That(result.RegionDataPointId, Is.EqualTo(minListing.RegionDataPointId));
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        
        if (_postgresContainer != null)
            await _postgresContainer.DisposeAsync();
    }

    private GilGoblinDbContext GetNewDbContext() => new(_options, _configuration);
    private string GetConnectionString() => _postgresContainer!.GetConnectionString();
}