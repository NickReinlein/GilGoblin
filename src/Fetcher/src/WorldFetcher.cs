using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.Fetcher.Pocos;

namespace GilGoblin.Fetcher;

public interface IWorldFetcher
{
    Task<List<WorldWebPoco>> GetAllAsync();
}

public class WorldFetcher(ILogger<WorldFetcher> Logger, HttpClient? Client = null) : IWorldFetcher
{
    public static string WorldBaseUrl => "https://universalis.app/api/v2/worlds";

    public async Task<List<WorldWebPoco>> GetAllAsync()
    {
        try
        {
            Client ??= new HttpClient();
            var response = await Client.GetAsync(WorldBaseUrl);
            return await ReadResponseContentAsync(response);
        }
        catch
        {
            Logger.LogError("Failed GET call to update all worlds with path: {Path}", WorldBaseUrl);
            return [];
        }
    }

    private static async Task<List<WorldWebPoco>> ReadResponseContentAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return [];

        try
        {
            return await response.Content.ReadFromJsonAsync<List<WorldWebPoco>>() ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}