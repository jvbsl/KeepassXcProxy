using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcTestAssociate : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "test-associate";
    public KeepassXcTestAssociate()
        : base(ActionName)
    {
    }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName(("key"))]
    public byte[] Key { get; set; }
}