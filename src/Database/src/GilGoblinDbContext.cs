using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Database;

public class GilGoblinDbContext : DbContext
{
    private readonly DbContextOptions<GilGoblinDbContext> _options;
    private readonly IConfiguration? _configuration;
    // private readonly IGilGoblinDatabaseInitializer? _initializer;

    public DbSet<ItemInfoPoco> ItemInfo { get; set; }
    public DbSet<PricePoco> Price { get; set; }
    public DbSet<RecipePoco> Recipe { get; set; }
    public DbSet<RecipeCostPoco> RecipeCost { get; set; }

    public GilGoblinDbContext(
        DbContextOptions<GilGoblinDbContext> options,
        IConfiguration? configuration = null)
        // IGilGoblinDatabaseInitializer? initializer = null)
        : base(options)
    {
        _options = options;
        _configuration = configuration;
        // _initializer = initializer;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration?.GetConnectionString(nameof(GilGoblinDbContext));
            optionsBuilder.UseSqlite(connectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemInfoPoco>().ToTable("ItemInfo");
        modelBuilder.Entity<ItemInfoPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Name);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.IconId);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Description);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.PriceMid);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.PriceLow);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.StackSize);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Level);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.CanBeHq);

        modelBuilder.Entity<PricePoco>().ToTable("Price");
        modelBuilder.Entity<PricePoco>().HasKey(t => new { t.ItemId, t.WorldId });
        modelBuilder.Entity<PricePoco>().Property(t => t.ItemId);
        modelBuilder.Entity<PricePoco>().Property(t => t.WorldId);
        modelBuilder.Entity<PricePoco>().Property(t => t.LastUploadTime);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPrice);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceHQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceNQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSold);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldHQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldNQ);

        modelBuilder.Entity<RecipePoco>().ToTable("Recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity);
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq);
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient0);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient1);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient2);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient3);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient4);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient5);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient6);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient7);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient8);
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient9);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient0TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient1TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient2TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient3TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient4TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient5TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient6TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient7TargetId);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient8TargetId);

        modelBuilder.Entity<RecipeCostPoco>().ToTable("RecipeCost");
        modelBuilder.Entity<RecipeCostPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecipeId);
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.WorldId);
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Cost);
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Created);
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Updated);
    }
}