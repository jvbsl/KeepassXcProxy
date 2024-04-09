using System.Text.Json;
using System.Text.Json.Serialization;
using Sodium;

namespace KeepassXcProxy;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcAssociate>), KeepassXcAssociate.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcGetDatabaseEntries>), KeepassXcGetDatabaseEntries.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcGetDatabaseGroups>), KeepassXcGetDatabaseGroups.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcGetDatabaseHash>), KeepassXcGetDatabaseHash.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcGetLogins>), KeepassXcGetLogins.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedAction<KeepassXcTestAssociate>), KeepassXcTestAssociate.ActionName)]

public class KeepassXcEncryptedAction : KeepassXcAction
{
    public KeepassXcEncryptedAction(string action) : base(action)
    {
    }
}

public class KeepassXcEncryptedAction<T> : KeepassXcEncryptedAction, IActionNamed
    where T : IActionNamed
{
    static string IActionNamed.ActionName => T.ActionName;
    
    public KeepassXcEncryptedAction(string clientId, byte[] nonce, byte[] message)
        : base(T.ActionName)
    {
        ClientId = clientId;
        Nonce = nonce;
        Message = message;
    }
    [JsonPropertyName("message")]
    public byte[] Message { get; }
    [JsonPropertyName("nonce")]
    public byte[] Nonce { get; }
    [JsonPropertyName("clientID")]
    public string ClientId { get; }

    public T GetMessage(byte[] secretKey, byte[] publicKey)
    {
        var decrypted = PublicKeyBox.Open(Message, Nonce, secretKey, publicKey);
        return JsonSerializer.Deserialize<T>(decrypted);
    }

    public static KeepassXcEncryptedAction<T> Create(string clientId, byte[] nonce, byte[] secretKey, byte[] publicKey, T action)
    {
        return new KeepassXcEncryptedAction<T>(clientId, nonce,
            PublicKeyBox.Create(JsonSerializer.Serialize(action), nonce, secretKey, publicKey));

    }
}