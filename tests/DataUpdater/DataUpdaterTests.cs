using System;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class DataUpdaterTests
{
    protected IServiceScope _scope;
    protected IServiceProvider _serviceProvider;
    protected IServiceScopeFactory _scopeFactory;

    [SetUp]
    public virtual void SetUp()
    {
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();

        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(_scopeFactory);
        _serviceProvider.CreateAsyncScope().Returns(_scope);
        _serviceProvider.CreateScope().Returns(_scope);
    }
}