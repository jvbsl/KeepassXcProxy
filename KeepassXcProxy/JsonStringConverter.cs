using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class JsonStringConverter<T> : JsonConverter<T>
    where T : IParsable<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Cannot convert token of type {reader.TokenType} to a {typeof(T)}. Expected string.");
        var value = reader.GetString()!;
        if (T.TryParse(value, null, out var res))
            return res;
        throw new JsonException($"Cannot convert value  '{value}' to a {typeof(T)}. ");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()?.ToLowerInvariant());
    }
}