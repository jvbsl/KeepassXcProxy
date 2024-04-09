using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcAssociateKey
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName(("key"))]
    public byte[] Key { get; set; }
}