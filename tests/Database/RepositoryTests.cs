using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class InMemoryTestDb
{
    protected DbContextOptions<GilGoblinDbContext> _options;
    protected IConfiguration _configuration;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _configuration = Substitute.For<IConfiguration>();

        _options = Substitute.For<DbContextOptions<GilGoblinDbContext>>();
        _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
            .UseInMemoryDatabase(databaseName: "GilGoblinTest")
            .Options;
    }
}