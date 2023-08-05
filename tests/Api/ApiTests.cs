using System.Net;
using Microsoft.AspNetCore.Builder;
using NUnit.Framework;

namespace GilGoblin.Tests.Api;

[NonParallelizable]
public class EndpointTests
{
    private WebApplicationBuilder _builder;
    private WebApplication _app;
    private HttpClient _client;

    [OneTimeSetUp]
    public void SetUp()
    {
        _builder = GilGoblin.Api.Startup.GetGoblinBuilder(null);
        _app = _builder.Build();
        _app.Run();
        _client = new HttpClient { Timeout = new TimeSpan(0, 0, 3) };
    }

    [TestCase("/item/")]
    [TestCase("/item/100")]
    [TestCase("/recipe/")]
    [TestCase("/recipe/100")]
    [TestCase("/price/34/")]
    [TestCase("/price/34/100")]
    [TestCase("/craft/34/")]
    [TestCase("/craft/34/100")]
    public async Task WhenWeResolveEndpoints_ThenEachEndpointResponds(string endpoint)
    {
        using var response = await _client.GetAsync(endpoint);
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(GetAcceptableResponseCodes(), Does.Contain(response.StatusCode));
        });
    }

    private static HttpStatusCode[] GetAcceptableResponseCodes() =>
        new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
}
