using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(AllEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"http://localhost:55448{endpoint}";

        using var response = await _client.GetAsync(fullEndpoint);

        response.EnsureSuccessStatusCode();
    }

    private static string[] AllEndPoints =
    {
        "/recipe/", "/recipe/100", "/price/34/", "/price/34/100",
        // "/craft/34/", "temporary for performance
        "/craft/34/100", "/item/", "/item/100"
    };
}