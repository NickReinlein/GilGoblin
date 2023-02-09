using System.Data.Common;
using GilGoblin.Pocos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin.Database;

public class GilGoblinDbContext : DbContext
{
    public DbSet<PricePoco>? Price { get; set; }
    public DbSet<ItemInfoPoco>? ItemInfo { get; set; }
    public DbSet<RecipePoco>? Recipe { get; set; }

    private SqliteConnection? _conn;

    public GilGoblinDbContext()
        : base(
            new DbContextOptionsBuilder<GilGoblinDbContext>()
                .UseSqlite<GilGoblinDbContext>(GoblinDatabase.Connect())
                .Options
        ) { }

    public GilGoblinDbContext(DbConnection connection)
        : base(
            new DbContextOptionsBuilder<GilGoblinDbContext>()
                .UseSqlite<GilGoblinDbContext>(connection)
                .Options
        ) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _conn = GoblinDatabase.Connect();
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
        modelBuilder.Entity<PricePoco>().HasKey(t => new { t.ItemID, t.WorldID });

        modelBuilder.Entity<ItemInfoPoco>().ToTable("ItemInfo");
        modelBuilder.Entity<ItemInfoPoco>().HasKey(t => t.ID);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Name);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.IconID);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Description);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.VendorPrice);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.StackSize);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Level);
        modelBuilder.Entity<ItemInfoPoco>().Property(t => t.CanBeHq);

        modelBuilder.Entity<RecipePoco>().ToTable("Recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.ID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity);
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemID);
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
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient0TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient1TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient2TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient3TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient4TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient5TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient6TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient7TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient8TargetID);
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient9TargetID);
    }
}
