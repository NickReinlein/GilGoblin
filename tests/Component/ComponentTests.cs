using System.Text.Json;
using GilGoblin.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ComponentTests
{
    protected HttpClient _client;
    protected WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Startup>();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    protected const double missingEntryPercentageThreshold = 0.85;

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
}