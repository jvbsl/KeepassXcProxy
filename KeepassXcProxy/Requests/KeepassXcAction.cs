using System.Text.Json.Serialization;

namespace KeepassXcProxy;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(KeepassXcAssociate), KeepassXcAssociate.ActionName)]
[JsonDerivedType(typeof(KeepassXcChangePublicKeys), KeepassXcChangePublicKeys.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseEntries), KeepassXcGetDatabaseEntries.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseGroups), KeepassXcGetDatabaseGroups.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetDatabaseHash), KeepassXcGetDatabaseHash.ActionName)]
[JsonDerivedType(typeof(KeepassXcGetLogins), KeepassXcGetLogins.ActionName)]
[JsonDerivedType(typeof(KeepassXcTestAssociate), KeepassXcTestAssociate.ActionName)]
public abstract class KeepassXcAction
{
    public KeepassXcAction(string action)
    {
        Action = action;
    }
    [JsonPropertyName("action")]
    public string Action { get; }
    
    [JsonPropertyName("triggerUnlock")]
    public bool? TriggerUnlock { get; set; }
}