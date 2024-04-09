using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcChangePublicKeys : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "change-public-keys";
    
    public KeepassXcChangePublicKeys(byte[] publicKey, byte[] nonce, string clientId)
        : base(ActionName)
    {
        PublicKey = publicKey;
        Nonce = nonce;
        ClientId = clientId;
    }

    [JsonPropertyName("publicKey")]
    public byte[] PublicKey { get; set; }

    [JsonPropertyName("nonce")]
    public byte[] Nonce { get; set; }

    [JsonPropertyName("clientID")]
    public string ClientId { get; set; }
}