using System;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Database;

public class GilGoblinDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<ItemPoco> Item { get; init; }
    public DbSet<WorldPoco> World { get; init; }
    public DbSet<RecipePoco> Recipe { get; init; }
    public DbSet<RecipeCostPoco> RecipeCost { get; init; }
    public DbSet<RecipeProfitPoco> RecipeProfit { get; init; }
    public DbSet<MinListingPoco> MinListing { get; init; }
    public DbSet<AverageSalePricePoco> AverageSalePrice { get; init; }
    public DbSet<RecentPurchasePoco> RecentPurchase { get; init; }
    public DbSet<DailySaleVelocityPoco> DailySaleVelocity { get; init; }
    public DbSet<WorldUploadTimeDbPoco> WorldUploadTime { get; init; }
    public DbSet<PricePoco> Price { get; init; }
    public DbSet<PriceDataPoco> PriceData { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable(nameof(GilGoblinDbContext))
                               ?? configuration.GetConnectionString(nameof(GilGoblinDbContext))
                               ?? throw new InvalidOperationException(
                                   "Connection string not found in environment variables or configuration.");
        optionsBuilder.UseNpgsql(connectionString);
        // Only used during development and debugging
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorldPoco>().ToTable("world");
        modelBuilder.Entity<WorldPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<WorldPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<WorldPoco>().Property(t => t.Name).HasColumnName("name");

        modelBuilder.Entity<ItemPoco>().ToTable("item");
        modelBuilder.Entity<ItemPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<ItemPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
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
        modelBuilder.Entity<RecipePoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<RecipePoco>().Property(t => t.CraftType).HasColumnName("craft_type");
        modelBuilder.Entity<RecipePoco>().Property(t => t.RecipeLevelTable).HasColumnName("recipe_level_table");
        modelBuilder.Entity<RecipePoco>().Property(t => t.TargetItemId).HasColumnName("target_item_id");
        modelBuilder.Entity<RecipePoco>().Property(t => t.ResultQuantity).HasColumnName("result_quantity");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanHq).HasColumnName("can_hq");
        modelBuilder.Entity<RecipePoco>().Property(t => t.CanQuickSynth).HasColumnName("can_quick_synth");

        // Ingredient Properties
        for (var i = 0; i <= 9; i++)
        {
            modelBuilder.Entity<RecipePoco>()
                .Property<int>($"ItemIngredient{i}TargetId")
                .HasColumnName($"item_ingredient{i}_target_id");
            modelBuilder.Entity<RecipePoco>()
                .Property<int>($"AmountIngredient{i}")
                .HasColumnName($"amount_ingredient{i}");
        }

        modelBuilder.Entity<PriceDataPoco>().ToTable("price_data");
        modelBuilder.Entity<PriceDataPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<PriceDataPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<PriceDataPoco>().Property(t => t.PriceType).HasColumnName("price_type");
        modelBuilder.Entity<PriceDataPoco>().Property(t => t.Price).HasColumnName("price");
        modelBuilder.Entity<PriceDataPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<PriceDataPoco>().Property(t => t.Timestamp).HasColumnName("timestamp");

        modelBuilder.Entity<DailySaleVelocityPoco>().ToTable("daily_sale_velocity");
        modelBuilder.Entity<DailySaleVelocityPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.World).HasColumnName("world_quantity");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.Dc).HasColumnName("dc_quantity");
        modelBuilder.Entity<DailySaleVelocityPoco>().Property(t => t.Region).HasColumnName("region_quantity");
        modelBuilder.Entity<DailySaleVelocityPoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_daily_sale_velocity_item_id_and_world_id_and_is_hq")
            .IsUnique();

        modelBuilder.Entity<MinListingPoco>().ToTable("min_listing");
        modelBuilder.Entity<MinListingPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<MinListingPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<MinListingPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.WorldDataPointId).HasColumnName("world_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.DcDataPointId).HasColumnName("dc_data_point_id");
        modelBuilder.Entity<MinListingPoco>().Property(t => t.RegionDataPointId).HasColumnName("region_data_point_id");
        modelBuilder.Entity<MinListingPoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_min_listing_item_id_and_world_id_and_is_hq")
            .IsUnique();
        modelBuilder.Entity<MinListingPoco>()
            .HasOne(p => p.DcDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MinListingPoco>()
            .HasOne(p => p.RegionDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MinListingPoco>()
            .HasOne(p => p.WorldDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AverageSalePricePoco>().ToTable("average_sale_price");
        modelBuilder.Entity<AverageSalePricePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.WorldDataPointId)
            .HasColumnName("world_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.DcDataPointId)
            .HasColumnName("dc_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>().Property(t => t.RegionDataPointId)
            .HasColumnName("region_data_point_id");
        modelBuilder.Entity<AverageSalePricePoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_average_sale_price_item_id_and_world_id_and_is_hq")
            .IsUnique();
        modelBuilder.Entity<AverageSalePricePoco>()
            .HasOne(p => p.DcDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<AverageSalePricePoco>()
            .HasOne(p => p.RegionDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<AverageSalePricePoco>()
            .HasOne(p => p.WorldDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecentPurchasePoco>().ToTable("recent_purchase");
        modelBuilder.Entity<RecentPurchasePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.WorldDataPointId)
            .HasColumnName("world_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.DcDataPointId)
            .HasColumnName("dc_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>().Property(t => t.RegionDataPointId)
            .HasColumnName("region_data_point_id");
        modelBuilder.Entity<RecentPurchasePoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_recent_purchase_item_id_and_world_id_and_is_hq")
            .IsUnique();
        modelBuilder.Entity<RecentPurchasePoco>()
            .HasOne(p => p.DcDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RecentPurchasePoco>()
            .HasOne(p => p.RegionDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RecentPurchasePoco>()
            .HasOne(p => p.WorldDataPoint)
            .WithOne()
            .HasForeignKey<PriceDataPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PricePoco>().ToTable("price");
        modelBuilder.Entity<PricePoco>().HasKey(t => t.Id);
        modelBuilder.Entity<PricePoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<PricePoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<PricePoco>().Property(t => t.MinListingId).HasColumnName("min_listing_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.RecentPurchaseId).HasColumnName("recent_purchase_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.AverageSalePriceId).HasColumnName("average_sale_price_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.DailySaleVelocityId).HasColumnName("daily_sale_velocity_id");
        modelBuilder.Entity<PricePoco>().Property(t => t.Updated).HasColumnName("updated");
        modelBuilder.Entity<PricePoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId })
            .HasDatabaseName("idx_price_item_id_and_world_id");
        modelBuilder.Entity<PricePoco>()
            .HasIndex(t => new { t.ItemId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_price_item_id_and_world_id_and_is_hq")
            .IsUnique();
        modelBuilder.Entity<PricePoco>()
            .HasOne(p => p.AverageSalePrice)
            .WithOne()
            .HasForeignKey<AverageSalePricePoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>()
            .HasOne(p => p.MinListing)
            .WithOne()
            .HasForeignKey<MinListingPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>()
            .HasOne(p => p.RecentPurchase)
            .WithOne()
            .HasForeignKey<RecentPurchasePoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PricePoco>()
            .HasOne(p => p.DailySaleVelocity)
            .WithOne()
            .HasForeignKey<DailySaleVelocityPoco>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorldUploadTimeDbPoco>().ToTable("world_upload_times");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.ItemId).HasColumnName("item_id");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<WorldUploadTimeDbPoco>().Property(t => t.Timestamp).HasColumnName("timestamp");

        modelBuilder.Entity<RecipeCostPoco>().ToTable("recipe_cost");
        modelBuilder.Entity<RecipeCostPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.Amount).HasColumnName("amount");
        modelBuilder.Entity<RecipeCostPoco>().Property(t => t.LastUpdated).HasColumnName("last_updated");
        modelBuilder.Entity<RecipeCostPoco>()
            .HasIndex(t => new { t.RecipeId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_recipe_cost_recipe_and_world_id_and_is_hq")
            .IsUnique();

        modelBuilder.Entity<RecipeProfitPoco>().ToTable("recipe_profit");
        modelBuilder.Entity<RecipeProfitPoco>().HasKey(t => t.Id);
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.RecipeId).HasColumnName("recipe_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.WorldId).HasColumnName("world_id");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.IsHq).HasColumnName("is_hq");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.Amount).HasColumnName("amount");
        modelBuilder.Entity<RecipeProfitPoco>().Property(t => t.LastUpdated).HasColumnName("last_updated");
        modelBuilder.Entity<RecipeProfitPoco>()
            .HasIndex(t => new { t.RecipeId, t.WorldId, t.IsHq })
            .HasDatabaseName("idx_recipe_profit_recipe_and_world_id_and_is_hq")
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}