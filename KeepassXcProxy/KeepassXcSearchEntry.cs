using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcSearchEntry
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
}