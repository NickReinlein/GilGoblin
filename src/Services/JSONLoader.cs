using System.Text.Json;

namespace GilGoblin.Services;

public static class JSONLoader
{
    public static async Task<IEnumerable<T>> LoadJSONFile<T>(string path) where T : class
    {
        var jsonData = await File.ReadAllTextAsync(path);
        if (string.IsNullOrWhiteSpace(jsonData))
            return Array.Empty<T>();

        var result = JsonSerializer.Deserialize<IEnumerable<T>>(jsonData) ?? Array.Empty<T>();
        return result;
    }
}
