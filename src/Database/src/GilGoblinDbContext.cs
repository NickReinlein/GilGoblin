using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Database;

public class GilGoblinDbContext : DbContext
{
    protected readonly DbContextOptions Options;
    protected readonly IConfiguration Configuration;

    public DbSet<ItemPoco> Item { get; set; }
    public DbSet<PricePoco> Price { get; set; }
    public DbSet<RecipePoco> Recipe { get; set; }
    public DbSet<RecipeCostPoco> RecipeCost { get; set; }
    public DbSet<RecipeProfitPoco> RecipeProfit { get; set; }
    public DbSet<WorldPoco> World { get; set; }

    public GilGoblinDbContext(DbContextOptions options, IConfiguration configuration)
        : base(options)
    {
        Options = options;
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration.GetConnectionString(nameof(GilGoblinDbContext));
        optionsBuilder.UseNpgsql(connectionString);

        // Only used during development and debugging
        // optionsBuilder.EnableSensitiveDataLogging();
        // optionsBuilder.EnableDetailedErrors();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorldPoco>().ToTable("world");
        modelBuilder.Entity<WorldPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<WorldPoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<WorldPoco>().Property(t => t.Name).HasColumnName("name");

        modelBuilder.Entity<ItemPoco>().ToTable("item");
        modelBuilder.Entity<ItemPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<ItemPoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<ItemPoco>().Property(t => t.Name).HasColumnName("name");
        modelBuilder.Entity<ItemPoco>().Property(t => t.Description).HasColumnName("description");
        modelBuilder.Entity<ItemPoco>().Property(t => t.IconId).HasColumnName("iconid");
        modelBuilder.Entity<ItemPoco>().Property(t => t.Level).HasColumnName("level");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceMid).HasColumnName("pricemid");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceLow).HasColumnName("pricelow");
        modelBuilder.Entity<ItemPoco>().Property(t => t.StackSize).HasColumnName("stacksize");
        modelBuilder.Entity<ItemPoco>().Property(t => t.CanHq).HasColumnName("canhq");

        modelBuilder.Entity<PricePoco>().ToTable("price");
        modelBuilder.Entity<PricePoco>().HasKey(t => new { t.ItemId, t.WorldId });
        modelBuilder.Entity<PricePoco>().Property(t => t.ItemId).HasColumnName("itemid");
        modelBuilder.Entity<PricePoco>().Property(t => t.WorldId).HasColumnName("worldid");
        modelBuilder.Entity<PricePoco>().Property(t => t.LastUploadTime).HasColumnName("lastuploadtime");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPrice).HasColumnName("averagelistingprice");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceHQ).HasColumnName("averagelistingpricehq");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageListingPriceNQ).HasColumnName("averagelistingpricenq");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSold).HasColumnName("averagesold");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldHQ).HasColumnName("averagesoldhq");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSoldNQ).HasColumnName("averagesoldnq");

        modelBuilder.Entity<RecipePoco>().ToTable("recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipePoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CraftType).HasColumnName("crafttype");
        modelBuilder.Entity<RecipePoco>().Property(t => t.RecipeLevelTable).HasColumnName("recipeleveltable");
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemId).HasColumnName("targetitemid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity).HasColumnName("resultquantity");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq).HasColumnName("canhq");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth).HasColumnName("canquicksynth");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient0TargetId)
            .HasColumnName("itemingredient0targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient0).HasColumnName("amountingredient0");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient1TargetId)
            .HasColumnName("itemingredient1targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient1).HasColumnName("amountingredient1");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient2TargetId)
            .HasColumnName("itemingredient2targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient2).HasColumnName("amountingredient2");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient3TargetId)
            .HasColumnName("itemingredient3targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient3).HasColumnName("amountingredient3");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient4TargetId)
            .HasColumnName("itemingredient4targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient4).HasColumnName("amountingredient4");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient5TargetId)
            .HasColumnName("itemingredient5targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient5).HasColumnName("amountingredient5");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient6TargetId)
            .HasColumnName("itemingredient6targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient6).HasColumnName("amountingredient6");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient7TargetId)
            .HasColumnName("itemingredient7targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient7).HasColumnName("amountingredient7");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient8TargetId)
            .HasColumnName("itemingredient8targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient8).HasColumnName("amountingredient8");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ItemIngredient9TargetId)
            .HasColumnName("itemingredient9targetid");
        modelBuilder.Entity<RecipePoco>().Property(t => t.AmountIngredient9).HasColumnName("amountingredient9");

        modelBuilder.Entity<RecipeCostPoco>().ToTable("recipecost");
        modelBuilder.Entity<RecipeCostPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecipeId).HasColumnName("recipeid");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.WorldId).HasColumnName("worldid");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Cost).HasColumnName("cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Updated).HasColumnName("updated");

        modelBuilder.Entity<RecipeProfitPoco>().ToTable("recipeprofit");
        modelBuilder.Entity<RecipeProfitPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecipeId).HasColumnName("recipeid");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.WorldId).HasColumnName("worldid");
        modelBuilder
            .Entity<RecipeProfitPoco>()
            .Property(t => t.RecipeProfitVsSold)
            .HasColumnName("profitvssold");
        modelBuilder
            .Entity<RecipeProfitPoco>()
            .Property(t => t.RecipeProfitVsListings)
            .HasColumnName("profitvslistings");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.Updated).HasColumnName("updated");
    }
}