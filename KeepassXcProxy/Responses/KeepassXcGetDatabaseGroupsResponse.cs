using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGetDatabaseGroupsResponse : KeepassXcActionResponse
{
    [JsonPropertyName("groups")]
    [JsonConverter(typeof(JsonRootGroupConverter))]
    public KeepassXcGroup Groups { get; set; }
}