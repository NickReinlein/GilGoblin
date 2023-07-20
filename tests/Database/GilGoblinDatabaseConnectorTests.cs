using GilGoblin.Database;
using NUnit.Framework;

namespace GilGoblin.Tests.Database;

public class GilGoblinDatabaseConnectorTests
{
    [Test]
    public void GivenAConnector_WhenConnectionExists_ThenItIsReturned()
    {
        var result = GilGoblinDatabaseConnector.Connect();

        Assert.That(result, Is.Not.Null);
    }
}
