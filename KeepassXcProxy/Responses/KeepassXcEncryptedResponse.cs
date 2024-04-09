using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sodium;

namespace KeepassXcProxy;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcAssociateResponse>), KeepassXcAssociate.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcGetDatabaseEntriesResponse>), KeepassXcGetDatabaseEntries.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcGetDatabaseGroupsResponse>), KeepassXcGetDatabaseGroups.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcGetDatabaseHashResponse>), KeepassXcGetDatabaseHash.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcGetLoginsResponse>), KeepassXcGetLogins.ActionName)]
[JsonDerivedType(typeof(KeepassXcEncryptedResponse<KeepassXcTestAssociateResponse>), KeepassXcTestAssociate.ActionName)]
public class KeepassXcEncryptedResponse : KeepassXcBaseResponse
{
    [JsonPropertyName("action")]
    public string Action { get; set; }
    
    [JsonPropertyName("nonce")]
    public byte[] Nonce { get; set; }
    [JsonPropertyName("message")]
    public byte[] Message { get; set; }

    public virtual KeepassXcMessage GetMessage(byte[] nonce, byte[] secretKey, byte[] publicKey)
    {
        throw new NotSupportedException();
    }
}


public class KeepassXcEncryptedResponse<T> : KeepassXcEncryptedResponse
    where T : KeepassXcMessage
{
    public override T GetMessage(byte[] nonce, byte[] secretKey, byte[] publicKey)
    {
        var decrypted = PublicKeyBox.Open(Message, nonce, secretKey, publicKey);
        return JsonSerializer.Deserialize<T>(decrypted, new JsonSerializerOptions(JsonSerializerDefaults.General)
                                                        {
                                                            Converters = { new JsonResponseConverter(), new JsonStringConverter<bool>(), new JsonStringConverter<int>() }
                                                        });
    }
}