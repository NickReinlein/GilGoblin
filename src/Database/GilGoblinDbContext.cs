using GilGoblin.Pocos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin.Database;

public class GilGoblinDbContext : DbContext
{
    public DbSet<PricePoco>? Price { get; set; }
    public DbSet<ItemInfoPoco>? ItemInfo { get; set; }
    public DbSet<RecipePoco>? Recipe { get; set; }
    public DbSet<IngredientPoco>? Ingredient { get; set; }

    private SqliteConnection? _conn;

    public GilGoblinDbContext()
        : base(
            new DbContextOptionsBuilder<GilGoblinDbContext>()
                .UseSqlite(GoblinDatabase.Connect())
                .Options
        ) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        this._conn = GoblinDatabase.Connect();
        optionsBuilder.UseSqlite(_conn);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PricePoco>().ToTable("Price");
        modelBuilder.Entity<PricePoco>().Property(t => t.ItemID);
        modelBuilder.Entity<PricePoco>().Property(t => t.WorldID);
        modelBuilder.Entity<PricePoco>().Property(t => t.LastUploadTime);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPrice);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceHQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceNQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSold);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldHQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldNQ);
        modelBuilder.Entity<PricePoco>().Property(t => t.Name);
        modelBuilder.Entity<PricePoco>().Property(t => t.RegionName);
        modelBuilder.Entity<PricePoco>().HasKey(t => new { t.ItemID, t.WorldID });

        modelBuilder.Entity<ItemInfoPoco>().ToTable("ItemInfo");
        modelBuilder.Entity<ItemInfoPoco>().HasKey(t => t.ID);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Name);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Icon);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Description);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.VendorPrice);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.StackSize);

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
