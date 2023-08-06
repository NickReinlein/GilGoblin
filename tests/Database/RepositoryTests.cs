using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RepositoryTests
{
    protected DbContextOptions<GilGoblinDbContext> _options;
    protected IConfiguration _configuration;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _configuration = Substitute.For<IConfiguration>();

        _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
}
