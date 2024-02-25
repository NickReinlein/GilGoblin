using System.Net.Http;
using System.Text.Json;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class ComponentTests
{
    protected const string baseUrl = "http://localhost:55448";
    protected HttpClient _client;

    [OneTimeSetUp]
    public virtual void OneTimeSetUp()
    {
        _client = new HttpClient();
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown()
    {
        _client?.Dispose();
    }

    protected const double missingEntryPercentageThreshold = 0.85;

    protected static JsonSerializerOptions GetSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true, };
}