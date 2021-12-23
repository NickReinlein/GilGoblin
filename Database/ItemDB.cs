using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Database
{
    internal class ItemDB
    {
        public MarketDataDB marketData { get; set; }
        public ItemInfo itemInfo { get; set; }

        public ItemDB() { }
    }

    internal class ItemDBContext : DbContext
    {
        public DbSet<ItemDB> data { get; set; }
        private SqliteConnection conn;

        public ItemDBContext()
            : base(new DbContextOptionsBuilder<ItemDBContext>().UseSqlite(DatabaseAccess.Connect()).Options)
        {
            conn = DatabaseAccess.Connect();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.conn = DatabaseAccess.Connect();
            optionsBuilder.UseSqlite(this.conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
            modelBuilder.Entity<MarketDataDB>().Property(t => t.item_id);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.world_id);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.last_updated);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.average_price);
            modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.item_id, t.world_id });
            modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);


            modelBuilder.Entity<MarketListingDB>().ToTable("MarketListing");
            modelBuilder.Entity<MarketListingDB>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.item_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.world_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.timestamp);
            modelBuilder.Entity<MarketListingDB>().Property(t => t.price).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.hq).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.qty).IsRequired();


            modelBuilder.Entity<ItemInfo>().ToTable("ItemInfo");
            modelBuilder.Entity<ItemInfo>().HasKey(t => t.item_id);
            modelBuilder.Entity<ItemInfo>().Property(t => t.name);
            modelBuilder.Entity<ItemInfo>().Property(t => t.icon_id);
            modelBuilder.Entity<ItemInfo>().Property(t => t.description);
            modelBuilder.Entity<ItemInfo>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfo>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfo>().Property(t => t.gathering_id);
            modelBuilder.Entity<ItemInfo>().HasMany(t => t.recipes);


            modelBuilder.Entity<RecipeDB>().ToTable("RecipeDB");
            modelBuilder.Entity<RecipeDB>().HasKey(t => t.recipe_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.result_quantity);
            modelBuilder.Entity<RecipeDB>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.target_item_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeDB>().HasMany(t => t.ingredients);


            modelBuilder.Entity<RecipeFullWeb>().ToTable("RecipeWeb");
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.ID).ValueGeneratedOnAdd(); ;
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.recipe_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.result_quantity).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.target_item_id).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeFullWeb>().OwnsMany(t => t.ingredients);


            modelBuilder.Entity<ItemRecipeShortAPI>().ToTable("ItemRecipeAPI");
            modelBuilder.Entity<ItemRecipeShortAPI>().Property(t => t.ID).ValueGeneratedOnAdd(); ;
            modelBuilder.Entity<ItemRecipeShortAPI>().Property(t => t.recipe_id).IsRequired();
            modelBuilder.Entity<ItemRecipeShortAPI>().Property(t => t.class_job_id).IsRequired();
            modelBuilder.Entity<ItemRecipeShortAPI>().Property(t => t.level).IsRequired();
        }
    }
}
