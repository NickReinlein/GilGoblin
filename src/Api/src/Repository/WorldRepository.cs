using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GilGoblin.Api.Repository;

public class WorldRepository : IWorldRepository
{
    private static ConcurrentDictionary<int, string>? AvailableWorlds { get; set; } = null;

    public Dictionary<int, string> GetAllWorlds()
    {
        var worldsDict = GetAvailableWorldsDict();
        return new Dictionary<int, string>(worldsDict);
    }

    public KeyValuePair<int, string> GetWorld(int id)
    {
        return GetAvailableWorldsDict().FirstOrDefault();
    }

    private static ConcurrentDictionary<int, string> GetAvailableWorldsDict()
    {
        if (AvailableWorlds is not null)
            return AvailableWorlds;

        var serverArray = DeserializeServerInfo(AllAvailableWorldsEntries.AllAvailableWorldsJson);
        var servers = new ConcurrentDictionary<int, string>();
        foreach (var server in serverArray)
        {
            servers.TryAdd(server.Key, server.Value);
        }

        return servers;
    }

    private static ConcurrentDictionary<int, string> DeserializeServerInfo(string json)
    {
        var serverDictionary = new ConcurrentDictionary<int, string>();
        var serverArray = JsonSerializer.Deserialize<ServerInfo[]>(json);

        foreach (var server in serverArray)
        {
            serverDictionary.TryAdd(server.id, server.name);
        }

        return serverDictionary;
    }
}

public record ServerInfo
{
    // ReSharper disable twice InconsistentNaming
    public int id { get; set; }
    public string name { get; set; }
}