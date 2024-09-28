using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GilGoblin.Fetcher.Pocos;

public class ItemWebPocoConverter : JsonConverter<ItemWebPoco>
{
    public override ItemWebPoco Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var id = root.GetProperty("ID").GetInt32();
        var name = root.GetProperty("Name").GetString();
        var description = root.GetProperty("Description").GetString();
        var iconId = root.GetProperty("IconID").GetInt32();
        var priceLow = root.GetProperty("PriceLow").GetInt32();
        var priceMid = root.GetProperty("PriceMid").GetInt32();
        var stackSize = root.GetProperty("StackSize").GetInt32();
        var level = root.GetProperty("LevelItem").GetInt32();
        var canBeHq = root.GetProperty("CanBeHq").GetInt32();

        return new ItemWebPoco(id, name!, description!, iconId, priceLow, priceMid, stackSize, level, canBeHq);
    }

    public override void Write(Utf8JsonWriter writer, ItemWebPoco value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}