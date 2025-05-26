using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class WorldTestBase : TestBase
{
    private const string worldEndpoint = "world/";

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAWorld(int worldId)
    {
        var fullEndpoint = $"{worldEndpoint}{worldId}";

        using var response = await _client.GetAsync(fullEndpoint);

        var world = await response.Content.ReadFromJsonAsync<WorldPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(world, Is.Not.Null);
            Assert.That(world!.Id, Is.EqualTo(worldId));
            Assert.That(world.Name, Has.Length.GreaterThan(0));
        });
    }

    [TestCase(10348555, HttpStatusCode.NoContent)]
    [TestCase(-10, HttpStatusCode.BadRequest)]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReturnBadRequest(
        int? worldId,
        HttpStatusCode expectedErrorCode)
    {
        var fullEndpoint = $"{worldEndpoint}{worldId}";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(expectedErrorCode));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllWorlds_ThenWeReceiveValidWorlds()
    {
        using var response = await _client.GetAsync(worldEndpoint);

        var worlds = 
            (await response.Content.ReadFromJsonAsync<IEnumerable<WorldPoco>>(GetSerializerOptions()) ?? []).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(worlds, Has.Count.GreaterThanOrEqualTo(2), "No worlds received");
            Assert.That(worlds.All(p => p.Id > 0), "Id is invalid");
            Assert.That(worlds.All(p => p.Name.Length > 0), "Name is invalid");
        });
    }
}