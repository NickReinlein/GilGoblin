using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(_allEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"http://localhost:55448{endpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        response.EnsureSuccessStatusCode();
    }

    private static string[] _allEndPoints =
    {
        "/recipe/", "/recipe/1639", "/price/34/", "/price/34/1639",
        // "/craft/34/", "temporary for performance
        "/craft/34/1639", "/item/", "/item/1639"
    };
}