using GilGoblin.Pocos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin.Database;

internal class GilGoblinDbContext : DbContext
{
    public DbSet<MarketDataPoco>? MarketData { get; set; }
    public DbSet<ItemInfoPoco>? ItemInfo { get; set; }
    public DbSet<RecipePoco>? Recipe { get; set; }
    public DbSet<IngredientPoco>? Ingredient { get; set; }

    private SqliteConnection? _conn;

    public GilGoblinDbContext()
        : base(
            new DbContextOptionsBuilder<GilGoblinDbContext>()
                .UseSqlite(GoblinDatabase.Connect())
                .Options
        )
    {
        _conn = GoblinDatabase.Connect();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        this._conn = GoblinDatabase.Connect();
        optionsBuilder.UseSqlite(_conn);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketDataPoco>().ToTable("MarketData");
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.ItemID);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.WorldID);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.LastUploadTime);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageListingPrice);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageListingPriceHQ);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageListingPriceNQ);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageSold);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageSoldHQ);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.AverageSoldNQ);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.Name);
        modelBuilder.Entity<MarketDataPoco>().Property(t => t.RegionName);
        modelBuilder.Entity<MarketDataPoco>().HasKey(t => new { t.ItemID, t.WorldID });

        modelBuilder.Entity<ItemInfoPoco>().ToTable("ItemInfo");
        modelBuilder.Entity<ItemInfoPoco>().HasKey(t => t.ID);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Name);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.IconID);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Description);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.VendorPrice);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.StackSize);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.GatheringID);

        modelBuilder.Entity<RecipePoco>().ToTable("Recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.RecipeID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity);
        modelBuilder.Entity<RecipePoco>().Property(t => t.IconID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq);
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth);
        modelBuilder.Entity<RecipePoco>().HasMany(t => t.Ingredients);

        modelBuilder.Entity<IngredientPoco>().ToTable("Ingredient");
        modelBuilder.Entity<IngredientPoco>().HasKey(t => t.ID);
        modelBuilder.Entity<IngredientPoco>().Property(t => t.RecipeID);
        modelBuilder.Entity<IngredientPoco>().Property(t => t.ItemID);
        modelBuilder.Entity<IngredientPoco>().Property(t => t.Quantity);
    }
}
