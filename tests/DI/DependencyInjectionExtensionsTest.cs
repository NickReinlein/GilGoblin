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
}
