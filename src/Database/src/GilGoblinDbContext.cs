using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Database
{
    public class GilGoblinDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GilGoblinDbContext> _logger;

        public DbSet<ItemInfoPoco> ItemInfo { get; set; }
        public DbSet<PricePoco> Price { get; set; }
        public DbSet<RecipePoco> Recipe { get; set; }
        public DbSet<RecipeCostPoco> RecipeCost { get; set; }

        public GilGoblinDbContext(
            IConfiguration configuration,
            ILogger<GilGoblinDbContext> logger,
            DbContextOptions<GilGoblinDbContext> options)
            : base(options)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
            optionsBuilder.UseNpgsql(connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemInfoPoco>().ToTable("ItemInfo");
            modelBuilder.Entity<ItemInfoPoco>().HasKey(t => t.Id);
            modelBuilder.Entity<ItemInfoPoco>().Property(t => t.Name);
            modelBuilder.Entity<ItemInfoPoco>().Property(t => t.IconId);

            modelBuilder.Entity<PricePoco>().ToTable("Price");
            modelBuilder.Entity<PricePoco>().HasKey(t => new { t.ItemId, t.WorldId });
            modelBuilder.Entity<PricePoco>().Property(t => t.ItemId);
            modelBuilder.Entity<PricePoco>().Property(t => t.WorldId);

            modelBuilder.Entity<RecipePoco>().ToTable("Recipe");
            modelBuilder.Entity<RecipePoco>().HasKey(t => t.Id);

            modelBuilder.Entity<RecipeCostPoco>().ToTable("RecipeCost");
            modelBuilder.Entity<RecipeCostPoco>().HasKey(t => new { t.RecipeId, t.WorldId });
        }
    }
}
