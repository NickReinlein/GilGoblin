using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Database;

public class GilGoblinDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    protected readonly IConfiguration Configuration = configuration;

    public DbSet<ItemPoco> Item { get; set; }
    public DbSet<WorldPoco> World { get; set; }
    public DbSet<RecipePoco> Recipe { get; set; }
    public DbSet<RecipeCostPoco> RecipeCost { get; set; }
    public DbSet<RecipeProfitPoco> RecipeProfit { get; set; }
    public DbSet<MinListingPoco> MinListing { get; set; }
    public DbSet<AverageSalePricePoco> AverageSalePrice { get; set; }
    public DbSet<RecentPurchasePoco> RecentPurchase { get; set; }
    public DbSet<DailySaleVelocityPoco> DailySaleVelocity { get; set; }
    public DbSet<WorldUploadTimeDbPoco> WorldUploadTime { get; set; }
    public DbSet<PricePoco> Price { get; set; }
    public DbSet<PriceDataDbPoco> PriceData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration.GetConnectionString(nameof(GilGoblinDbContext));
        optionsBuilder.UseNpgsql(connectionString);

        // Only used during development and debugging
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

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
        modelBuilder.Entity<ItemPoco>().Property(t => t.IconId).HasColumnName("icon_id");
        modelBuilder.Entity<ItemPoco>().Property(t => t.Level).HasColumnName("level");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceMid).HasColumnName("price_mid");
        modelBuilder.Entity<ItemPoco>().Property(t => t.PriceLow).HasColumnName("price_low");
        modelBuilder.Entity<ItemPoco>().Property(t => t.StackSize).HasColumnName("stack_size");
        modelBuilder.Entity<ItemPoco>().Property(t => t.CanHq).HasColumnName("can_hq");

        modelBuilder.Entity<RecipePoco>().ToTable("recipe");
        modelBuilder.Entity<RecipePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipePoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CraftType).HasColumnName("craft_type");
        modelBuilder.Entity<RecipePoco>().Property(t => t.RecipeLevelTable).HasColumnName("recipe_level_table");
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemId).HasColumnName("target_item_id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity).HasColumnName("result_quantity");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq).HasColumnName("can_hq");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth).HasColumnName("can_quick_synth");

        // Ingredient Properties
        for (int i = 0; i <= 9; i++)
        {
            modelBuilder.Entity<RecipePoco>()
                .Property<int>($"ItemIngredient{i}TargetId")
                .HasColumnName($"item_ingredient{i}_target_id");
            modelBuilder.Entity<RecipePoco>()
                .Property<int>($"AmountIngredient{i}")
                .HasColumnName($"amount_ingredient{i}");
        }

        modelBuilder.Entity<PriceDataDbPoco>().ToTable("price_data");
        modelBuilder.Entity<PriceDataDbPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<PriceDataDbPoco>().Property(t => t.Id).HasColumnName("id");
        modelBuilder.Entity<PriceDataDbPoco>().Property(t => t.PriceType).HasColumnName("price_type");
        modelBuilder.Entity<PriceDataDbPoco>().Property(t => t.Price).HasColumnName("price");
        modelBuilder.Entity<PriceDataDbPoco>().Property(t => t.Timestamp).HasColumnName("timestamp");
        modelBuilder.Entity<PriceDataDbPoco>().Property(t => t.WorldId).HasColumnName("world_id");

        modelBuilder.Entity<DailySaleVelocityPoco>().ToTable("daily_sale_velocity");
        modelBuilder.Entity<DailySaleVelocityPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.WorldQuantity).HasColumnName("world_quantity");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.DcQuantity).HasColumnName("dc_quantity");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.RegionQuantity).HasColumnName("region_quantity");

        modelBuilder.Entity<WorldUploadTimeDbPoco>().ToTable("world_upload_times");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.Timestamp).HasColumnName("timestamp");

        modelBuilder.Entity<MinListingPoco>().ToTable("min_listing");
        modelBuilder.Entity<MinListingPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<MinListingPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.WorldDataPointId).HasColumnName("world_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.RegionDataPointId).HasColumnName("region_data_point_id");

        modelBuilder.Entity<AverageSalePricePoco>().ToTable("average_sale_price");
        modelBuilder.Entity<AverageSalePricePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.WorldDataPointId).HasColumnName("world_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.RegionDataPointId).HasColumnName("region_data_point_id");

        modelBuilder.Entity<RecentPurchasePoco>().ToTable("recent_purchase");
        modelBuilder.Entity<RecentPurchasePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.WorldDataPointId).HasColumnName("world_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.RegionDataPointId).HasColumnName("region_data_point_id");

        modelBuilder.Entity<RecipeCostPoco>().ToTable("recipe_cost");
        modelBuilder.Entity<RecipeCostPoco>().HasKey(t => new { t.RecipeId, t.WorldId, t.IsHq });
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.AverageSaleCost).HasColumnName("average_sale_price_cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.MinListingCost).HasColumnName("min_listing_price_cost");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecentPurchaseCost).HasColumnName("recent_purchase_cost");

        modelBuilder.Entity<RecipeProfitPoco>().ToTable("recipe_profit");
        modelBuilder.Entity<RecipeProfitPoco>().HasKey(t => new { t.RecipeId, t.WorldId, t.IsHq });
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.AverageSaleProfit).HasColumnName("average_sale_price_profit");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.MinListingProfit).HasColumnName("min_listing_price_profit");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecentPurchaseProfit).HasColumnName("recent_purchase_profit");

        modelBuilder.Entity<PricePoco>().ToTable("price");
        modelBuilder.Entity<PricePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<PricePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<PricePoco>().Property(t => t.MinListingId).HasColumnName("min_listing_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.RecentPurchaseId).HasColumnName("recent_purchase_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSalePriceId).HasColumnName("average_sale_price_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.DailySaleVelocityId).HasColumnName("daily_sale_velocity_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.Updated).HasColumnName("updated");

        // Applying unique constraints, indexes, and relationships
        modelBuilder.Entity<PricePoco>().HasIndex(t => t.ItemId).HasDatabaseName("idx_price_item_id");
        modelBuilder.Entity<PricePoco>().HasIndex(t => new { t.ItemId, t.WorldId }).HasDatabaseName("idx_price_item_id_and_world_id");
        modelBuilder.Entity<PricePoco>().HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq }).HasDatabaseName("idx_price_item_id_and_world_id_and_is_hq");
        modelBuilder.Entity<PricePoco>().HasOne(p => p.MinListing).WithMany().HasForeignKey(t => t.MinListingId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>().HasOne(p => p.RecentPurchase).WithMany().HasForeignKey(t => t.RecentPurchaseId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>().HasOne(p => p.AverageSalePrice).WithMany().HasForeignKey(t => t.AverageSalePriceId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>().HasOne(p => p.DailySaleVelocity).WithMany().HasForeignKey(t => t.DailySaleVelocityId).OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}