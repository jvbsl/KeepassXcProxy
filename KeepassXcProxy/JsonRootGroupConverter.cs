using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class JsonRootGroupConverter : JsonConverter<KeepassXcGroup>
{
    public override KeepassXcGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject || !reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType != JsonTokenType.PropertyName || !reader.ValueTextEquals("groups") || !reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType != JsonTokenType.StartArray || !reader.Read())
        {
            throw new JsonException();
        }

        var res = JsonSerializer.Deserialize<KeepassXcGroup>(ref reader);
        
        if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return res;
    }

    public override void Write(Utf8JsonWriter writer, KeepassXcGroup value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStartArray("groups");
        JsonSerializer.Serialize(writer, value);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}