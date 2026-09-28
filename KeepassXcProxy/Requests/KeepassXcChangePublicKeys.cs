using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcChangePublicKeys(byte[] publicKey, byte[] nonce, string clientId) : KeepassXcAction(ActionName), IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "change-public-keys";

    [JsonPropertyName("publicKey")]
    public byte[] PublicKey { get; set; } = publicKey;

    [JsonPropertyName("nonce")]
    public byte[] Nonce { get; set; } = nonce;

    [JsonPropertyName("clientID")]
    public string ClientId { get; set; } = clientId;
}