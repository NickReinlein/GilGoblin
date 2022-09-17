using GilGoblin.Pocos;
using Microsoft.EntityFrameworkCore;

namespace GilGoblin.Database;

public class CraftingContext : DbContext
{
    public DbSet<MarketDataPoco> MarketData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketDataPoco>()
            .Property(md => md.ItemID)
            .IsRequired();
        modelBuilder.Entity<MarketDataPoco>()
            .Property(md => md.WorldID)
            .IsRequired();

    }
}