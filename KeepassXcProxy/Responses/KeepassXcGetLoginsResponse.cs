using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGetLoginsResponse : KeepassXcActionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
    [JsonPropertyName("entries")]
    public KeepassXcEntry[] Entries { get; set; }
}