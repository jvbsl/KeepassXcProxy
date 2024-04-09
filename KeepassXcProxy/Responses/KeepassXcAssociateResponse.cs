using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcAssociateResponse : KeepassXcActionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}