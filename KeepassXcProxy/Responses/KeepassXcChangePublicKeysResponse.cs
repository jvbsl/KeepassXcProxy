using System.Text.Json.Serialization;

namespace KeepassXcProxy;


public class KeepassXcChangePublicKeysResponse : KeepassXcActionResponse
{
    [JsonPropertyName("publicKey")]
    public byte[] PublicKey { get; set; }
}