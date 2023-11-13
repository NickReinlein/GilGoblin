using System;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant;

public class AccountantDependencyInjectionTests
{
    protected WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Startup>();
    }

    [Test]
    public void GivenAProgram_WhenStarting_ThenThereAreNoCompileErrors()
    {
        var client = _factory.CreateClient();

        Assert.That(client, Is.Not.Null);
    }

    [TestCase(typeof(IDataSaver<RecipeCostPoco>))]
    [TestCase(typeof(IDataSaver<RecipeProfitPoco>))]
    [TestCase(typeof(IAccountant<RecipeCostPoco>))]
    [TestCase(typeof(ICraftingCalculator))]
    public void GivenAGoblinService_WhenWeSetup_ThenTheServiceIsResolved(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}