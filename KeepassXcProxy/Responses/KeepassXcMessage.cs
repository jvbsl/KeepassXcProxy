using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcMessage : KeepassXcBaseResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("version")]
    public string Version { get; set; }
    [JsonPropertyName("nonce")]
    public byte[] Nonce { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(KeepassXcAssociateResponse), KeepassXcAssociate.ActionName)]
[JsonDerivedType(typeof(KeepassXcChangePublicKeysResponse), KeepassXcChangePublicKeys.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseEntriesResponse), KeepassXcGetDatabaseEntries.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseGroupsResponse), KeepassXcGetDatabaseGroups.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseHashResponse), KeepassXcGetDatabaseHash.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetLoginsResponse), KeepassXcGetLogins.ActionName)]
[JsonDerivedType(typeof(KeepassXcTestAssociateResponse), KeepassXcTestAssociate.ActionName)]
public class KeepassXcActionResponse : KeepassXcMessage
{
    [JsonPropertyName("action")]
    public string Action { get; set; }
}