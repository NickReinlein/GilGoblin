using System.Threading.Tasks;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class EndpointComponentTests : ComponentTests
{
    [TestCaseSource(nameof(_allEndPoints))]
    public async Task GivenACallToGet_WhenTheEndPointIsValid_ThenTheEndpointResponds(
        string endpoint
    )
    {
        var fullEndpoint = $"{baseUrl}{endpoint}";

        var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.IsSuccessStatusCode, $"{endpoint}");
    }

    private static string[] _allEndPoints =
    {
        "/recipe/", "/recipe/1639", "/price/34/", "/price/34/1639", "/craft/34/", "/item/", "/item/1639"
    };
}