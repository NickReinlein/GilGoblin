using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GilGoblin.Tests;

public class TestGilGoblinDbContext : GilGoblinDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "GilGoblinTest");
    }

    public TestGilGoblinDbContext(DbContextOptions options, IConfiguration configuration)
        : base(options, configuration)
    {
    }
}