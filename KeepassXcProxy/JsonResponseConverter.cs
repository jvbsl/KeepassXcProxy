using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class JsonResponseConverter : JsonConverter<KeepassXcBaseResponse>
{
    public override KeepassXcBaseResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonSerializer.Deserialize<JsonObject>(ref reader, options);
        if (jsonObject.ContainsKey("error"))
        {
            return jsonObject.Deserialize<KeepassXcErrorResponse>(options);
        }
        if (jsonObject.ContainsKey("message"))
        {
            return jsonObject.Deserialize<KeepassXcEncryptedResponse>(options);
        }
        if (jsonObject.ContainsKey("action"))
        {
            return jsonObject.Deserialize<KeepassXcActionResponse>(options);
        }

        return jsonObject.Deserialize<KeepassXcMessage>(options);
    }

    public override void Write(Utf8JsonWriter writer, KeepassXcBaseResponse value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}