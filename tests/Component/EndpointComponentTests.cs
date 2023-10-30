using NUnit.Framework;

namespace GilGoblin.Tests.Component;

[NonParallelizable]
public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(_allEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"http://localhost:55448{endpoint}";

        var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.IsSuccessStatusCode);
    }

    private static string[] _allEndPoints =
    {
        "/recipe/", "/recipe/1639", "/price/34/", "/price/34/1639",
        // "/craft/34/",  // temporarily disabled for performance 
        "/craft/34/1639", "/item/", "/item/1639"
    };
}