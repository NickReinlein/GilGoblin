using System.Net;
using GilGoblin.Api.DI;
using Microsoft.AspNetCore.Builder;
using NUnit.Framework;

namespace GilGoblin.Tests.Api;

[NonParallelizable]
public class AcceptableResponseCodes
{
    private WebApplicationBuilder _builder;
    private WebApplication _app;
    private HttpClient _client;

    [OneTimeSetUp]
    public void SetUp()
    {
        _builder = Array.Empty<string>().GetGoblinBuilder();
        _app = _builder.Build();
        // var port = 6200 + DateTime.Now.Millisecond;
        // _app.Urls.Add($"http://localhost:6222");
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

        Assert.That(response, Is.Not.Null);
        Assert.That(GetAcceptableResponseCodes().Contains(response.StatusCode), Is.True);
    }

    private static HttpStatusCode[] GetAcceptableResponseCodes() =>
        new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
}
