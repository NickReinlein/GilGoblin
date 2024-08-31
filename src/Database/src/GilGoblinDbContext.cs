using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Database;

public class GilGoblinDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    protected readonly DbContextOptions Options = options;
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<ItemPoco> Item { get; set; }
    public DbSet<WorldPoco> World { get; set; }
    public DbSet<RecipePoco> Recipe { get; set; }
    public DbSet<RecipeCostPoco> RecipeCost { get; set; }
    public DbSet<RecipeProfitPoco> RecipeProfit { get; set; }
    public DbSet<PriceDataPointsPoco> PriceDataPoints { get; set; }
    public DbSet<MinListingPoco> MinListing { get; set; }
    public DbSet<AverageSalePricePoco> AverageSalePrice { get; set; }
    public DbSet<RecentPurchasePoco> RecentPurchase { get; set; }

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
        modelBuilder.Entity<ItemPoco>().Property(t => t.Description).HasColumnName("description").HasDefaultValue("");
        modelBuilder.Entity<ItemPoco>().Property(t => t.IconId).HasColumnName("icon_id");
        modelBuilder.Entity<ItemPoco>().Property(t => t.Level).HasColumnName("level");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceMid).HasColumnName("price_mid");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceLow).HasColumnName("price_low");
        modelBuilder.Entity<ItemPoco>().Property(t => t.StackSize).HasColumnName("stack_size");
        modelBuilder.Entity<ItemPoco>().Property(t => t.CanHq).HasColumnName("can_hq");

        modelBuilder.Entity<PriceDataPointsPoco>().ToTable("price_data_points");
        modelBuilder.Entity<PriceDataPointsPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.Price).HasColumnName("price");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.DcId).HasColumnName("dc_id");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.RegionId).HasColumnName("region_id");
        modelBuilder.Entity<PriceDataPointsPoco>().Property(t => t.Timestamp).HasColumnName("timestamp");

        modelBuilder.Entity<MinListingPoco>().ToTable("min_listing");
        modelBuilder.Entity<MinListingPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<MinListingPoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.WorldDataPointId).HasColumnName("world_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.RegionDataPointId).HasColumnName("region_data_point_id");

        modelBuilder.Entity<AverageSalePricePoco>().ToTable("average_sale_price");
        modelBuilder.Entity<AverageSalePricePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.WorldDataPointId)
            .HasColumnName("world_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.RegionDataPointId)
            .HasColumnName("region_data_point_id");

        modelBuilder.Entity<RecentPurchasePoco>().ToTable("recent_purchase");
        modelBuilder.Entity<RecentPurchasePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.WorldDataPointId)
            .HasColumnName("world_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.RegionDataPointId)
            .HasColumnName("region_data_point_id");

        modelBuilder.Entity<RecipePoco>().ToTable("recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipePoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CraftType).HasColumnName("craft_type");
        modelBuilder.Entity<RecipePoco>().Property(t => t.RecipeLevelTable).HasColumnName("recipe_level_table");
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemId).HasColumnName("target_item_id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity).HasColumnName("result_quantity");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq).HasColumnName("can_hq");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth).HasColumnName("can_quick_synth");

        modelBuilder.Entity<RecipeCostPoco>().ToTable("recipecost");
        modelBuilder.Entity<RecipeCostPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.AverageSalePriceCost)
            .HasColumnName("average_sale_price_cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.MinListingPriceCost)
            .HasColumnName("min_listing_price_cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecentPurchaseCost).HasColumnName("recent_purchase_cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Updated).HasColumnName("updated");

        modelBuilder.Entity<RecipeProfitPoco>().ToTable("recipeprofit");
        modelBuilder.Entity<RecipeProfitPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.AverageSalePriceProfit)
            .HasColumnName("average_sale_price_profit");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.MinListingPriceProfit)
            .HasColumnName("min_listing_price_profit");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecentPurchasePrice)
            .HasColumnName("recent_purchase_price");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.Updated);
    }
}