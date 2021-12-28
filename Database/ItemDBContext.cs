using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin.Database
{
    internal class ItemDBContext : DbContext
    {
        public DbSet<ItemDB> data { get; set; }
        public DbSet<MarketDataDB> marketData { get; set; }
        public DbSet<ItemInfoDB> itemInfoData { get; set; }
        public DbSet<RecipeDB> recipeData { get; set; }

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
            /* General data for an item ID -- A main accessor class */
            modelBuilder.Entity<ItemDB>().ToTable("ItemDB");
            modelBuilder.Entity<ItemDB>().HasKey(t => t.itemID);
            modelBuilder.Entity<ItemDB>().HasOne(t => t.itemInfo);
            modelBuilder.Entity<ItemDB>().HasMany(t => t.marketData);
            modelBuilder.Entity<ItemDB>().HasMany(t => t.fullRecipes);


            /* General market data for an item ID & world (with calculated average price and more) */
            modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
            modelBuilder.Entity<MarketDataDB>().Property(t => t.itemID);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.worldID);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.lastUpdated);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.averagePrice);
            modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.itemID, t.worldID });
            modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);

            // All the market listings for an item & world ID
            modelBuilder.Entity<MarketListingDB>().ToTable("MarketListing");
            modelBuilder.Entity<MarketListingDB>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.item_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.world_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.timestamp);
            modelBuilder.Entity<MarketListingDB>().Property(t => t.price).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.hq).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.qty).IsRequired();

            //// Item info from the db with full recipes
            modelBuilder.Entity<ItemInfoDB>().ToTable("ItemInfoDB");
            modelBuilder.Entity<ItemInfoDB>().HasKey(t => t.itemID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.name);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.iconID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.description);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.gatheringID);
            modelBuilder.Entity<ItemInfoDB>().HasMany(t => t.fullRecipes);


            // Database format for the full recipes
            modelBuilder.Entity<RecipeDB>().ToTable("RecipeDB");
            modelBuilder.Entity<RecipeDB>().HasKey(t => t.recipe_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.result_quantity);
            modelBuilder.Entity<RecipeDB>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.target_item_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeDB>().HasMany(t => t.ingredients);


            // The full recipe from the web
            modelBuilder.Entity<RecipeFullWeb>().ToTable("RecipeFullWeb");
            modelBuilder.Entity<RecipeFullWeb>().HasKey(t => t.recipe_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.result_quantity).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.target_item_id).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeFullWeb>().HasMany(t => t.ingredients);


            // Item info from the web with short recipes
            modelBuilder.Entity<ItemInfoWeb>().ToTable("ItemInfoWeb");
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.itemID).IsRequired();
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.name);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.iconID);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.description);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.gatheringID);
            modelBuilder.Entity<ItemInfoWeb>().HasMany(t => t.recipeHeader);

            //The abbreviated recipe with only 3 properties in the market data
            modelBuilder.Entity<ItemRecipeHeaderAPI>().ToTable("ItemRecipeHeaderAPI");
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.ID).ValueGeneratedOnAdd(); ;
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.recipe_id).IsRequired();
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.class_job_id).IsRequired();
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.level).IsRequired();
        }
    }
}
