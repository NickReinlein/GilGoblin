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
        var hostBuilder = Bootstrap
            .CreateHostBuilder("local")
            .ConfigureServices((_, services) =>
            {
                var descriptor = new ServiceDescriptor(
                    typeof(IRecipeGateway),
                    typeof(RecipeGateway),
                    ServiceLifetime.Scoped);
                services.Replace(descriptor);
            }
        );
        var host = hostBuilder.Build();
        Assert.That(host, Is.Not.Null);
    }
}
