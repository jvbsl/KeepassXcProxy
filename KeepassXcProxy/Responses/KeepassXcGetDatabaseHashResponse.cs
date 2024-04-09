using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGetDatabaseHashResponse : KeepassXcActionResponse
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}