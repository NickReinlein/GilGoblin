using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using GilGoblin.Web;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GilGoblin.Tests.DI;

public class DependencyInjectionExtensionsTest
{
    [Test]
    public void GivenAHostBuilder_WhenWeSetupHost_ThenWeSucceed()
    {
        var hostBuilder = Bootstrap.CreateHostBuilder().Build();

        Assert.That(hostBuilder, Is.Not.Null);
    }

    [Test]
    public void GivenAGilGoblin_WhenWeRun_ThenWeSucceed()
    {
        var program = new GilGoblin();
        Assert.That(program is not null);
    }
}
