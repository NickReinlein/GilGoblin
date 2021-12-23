using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Database
{
    internal class ItemDB
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        public int itemId { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        public int worldId { get; set; }
        public MarketDataDB marketData { get; set; }
        public ItemInfoDB itemInfo { get; set; }
        public List<RecipeDB> recipes { get; set; } = new List<RecipeDB>();        

        public ItemDB( ) { }

        /// <summary>
        /// Searches the database for a bulk list of items given their ID & world
        /// </summary>
        /// <param name="itemIDList"></param>A List of integers to represent item ID
        /// <param name="worldId"></param>the world ID for world-specific data (ie: market price)
        /// <returns></returns>
        public static List<ItemDB> GetItemDataDBBulk(List<int> itemIDList, int worldId)
        {
            try
            {
                ItemDBContext ItemDBContext = new ItemDBContext();

                List<ItemDB> exists = ItemDBContext.data
                        .Where(t => (t.worldId == worldId &&
                                     itemIDList.Contains(t.itemId)))
                        //.Include(t => t.marketData.listings)
                        .ToList();
                if (exists != null) { return exists; }
                List<ItemDB> newData = new List<ItemDB>();
                foreach (int thisID in itemIDList) {

                    ItemInfoDB itemInfo = MarketDataWeb.GetItemInfo(thisID);         
                }
                //ItemDBContext.itemInfoData.AddRangeAsync(newData);
                ItemDBContext.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                if (ex is SqliteException || ex is InvalidOperationException)
                {
                    //Maybe the database doesn't exist yet or not found
                    //Either way, we can return null -> it is not on the database
                    Log.Debug("Database or entry does not exist.");
                    return null;
                }
                else
                {

                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("Inner exception:{inner}.", ex.InnerException);
                    DatabaseAccess.Disconnect();
                    return null; ;
                }
            }
        }

        /// <summary>
        /// Searches the database for MarketData (average market price, vendor price, etc)
        /// </summary>
        /// <param name="item_id"></param>item ID (ie: 5057 for Copper Ingot)
        /// <param name="world_id"></param>world ID (ie: 34 for Brynhildr)
        /// <returns></returns>
        public static ItemDB GetItemDataDB(int item_id, int world_id)
        {
            List<int> itemIDList = new List<int>();
            itemIDList.Add(item_id);
            return ItemDB.GetItemDataDBBulk(itemIDList, world_id).FirstOrDefault();
        }

    }

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
            /* General market data for an item ID & world (with calculated average price and more) */
            modelBuilder.Entity<ItemDB>().ToTable("ItemDB");
            modelBuilder.Entity<ItemDB>().Property(t => t.itemId);
            modelBuilder.Entity<ItemDB>().Property(t => t.worldId);

            //modelBuilder.Entity<ItemDB>().Property(t => t.itemInfo);
            //modelBuilder.Entity<ItemDB>().Property(t => t.marketData);
            //modelBuilder.Entity<ItemDB>().HasMany(t => t.recipes);

            modelBuilder.Entity<ItemDB>().HasKey(t => new { t.itemId, t.worldId });

            /* General market data for an item ID & world (with calculated average price and more) */
            modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
            modelBuilder.Entity<MarketDataDB>().Property(t => t.item_id);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.world_id);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.last_updated);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.average_price);
            modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.item_id, t.world_id });
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
            modelBuilder.Entity<ItemInfoDB>().ToTable("ItemInfo");
            modelBuilder.Entity<ItemInfoDB>().HasKey(t => t.itemID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.name);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.iconID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.description);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.gatheringID);
            //modelBuilder.Entity<ItemInfoDB>().HasMany(t => t.fullRecipes);


            // Database format for the recipe
            modelBuilder.Entity<RecipeDB>().ToTable("RecipeDB");
            modelBuilder.Entity<RecipeDB>().HasKey(t => t.recipe_id);            
            modelBuilder.Entity<RecipeDB>().Property(t => t.result_quantity);
            modelBuilder.Entity<RecipeDB>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.target_item_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeDB>().HasMany(t => t.ingredients);


            // The full recipe from the web
            modelBuilder.Entity<RecipeFullWeb>().ToTable("RecipeWeb");
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
