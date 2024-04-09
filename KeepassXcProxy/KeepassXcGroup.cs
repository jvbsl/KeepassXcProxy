using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGroup
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("children")]
    public KeepassXcGroup[] Children { get; set; }
}