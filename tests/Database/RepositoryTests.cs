using GilGoblin.Database;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class RepositoryTests
{
    protected DbContextOptions<GilGoblinDbContext> _options;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _options = new DbContextOptionsBuilder<GilGoblinDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
}
