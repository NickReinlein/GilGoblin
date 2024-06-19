using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GilGoblin.Fetcher.Pocos;

public class WorldWebPocoConverter : JsonConverter<WorldWebPoco>
{
    public override WorldWebPoco Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var id = root.GetProperty("ID").GetInt32();
        var name = root.GetProperty("Name").GetString();

        return new WorldWebPoco(id, name);
    }

    public override void Write(Utf8JsonWriter writer, WorldWebPoco value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}