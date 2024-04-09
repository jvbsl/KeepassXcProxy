using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcAssociate : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "associate";
    public KeepassXcAssociate()
        : base(ActionName)
    {
    }
    
    [JsonPropertyName("key")]
    public byte[] Key { get; set; }
    
    [JsonPropertyName(("idKey"))]
    public byte[] IdKey { get; set; }

}