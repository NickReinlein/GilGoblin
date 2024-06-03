using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using NUnit.Framework;

namespace GilGoblin.Tests.Component;

public class WorldComponentTests : ComponentTests
{
    private const int testWorldId = 34;

    [Test]
    public async Task GivenACallToGet_WhenTheInputIsValid_ThenWeReceiveAWorld()
    {
        var fullEndpoint = $"http://localhost:55448/world/{testWorldId}";

        using var response = await _client.GetAsync(fullEndpoint);

        var world = await response.Content.ReadFromJsonAsync<WorldPoco>(GetSerializerOptions());
        Assert.Multiple(() =>
        {
            Assert.That(world, Is.Not.Null);
            Assert.That(world!.Id, Is.EqualTo(testWorldId));
            Assert.That(world.Name, Has.Length.GreaterThan(0));
        });
    }

    [TestCase(10348555, HttpStatusCode.NoContent)]
    [TestCase(-10, HttpStatusCode.BadRequest)]
    public async Task GivenACallToGet_WhenTheInputIsInvalid_ThenWeReturnBadRequest(
        int? worldId,
        HttpStatusCode expectedErrorCode)
    {
        var fullEndpoint = $"http://localhost:55448/world/{worldId}";

        using var response = await _client.GetAsync(fullEndpoint);

        Assert.That(response.StatusCode, Is.EqualTo(expectedErrorCode));
    }

    [Test]
    public async Task GivenACallToGetAll_WhenReceivingAllWorlds_ThenWeReceiveValidWorlds()
    {
        const string fullEndpoint = "http://localhost:55448/World/";

        using var response = await _client.GetAsync(fullEndpoint);

        var worlds = (await response.Content.ReadFromJsonAsync<IEnumerable<WorldPoco>>(
            GetSerializerOptions()
        ))!.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(worlds, Has.Count.GreaterThan(0), "No worlds received");
            Assert.That(worlds.All(p => p.Id > 0), "Id is invalid");
            Assert.That(worlds.All(p => p.Name.Length > 0), "Name is invalid");
        });
    }
}