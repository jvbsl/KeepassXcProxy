using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGetLogins : KeepassXcAction, IActionNamed
{
    static string IActionNamed.ActionName => ActionName;
    public const string ActionName = "get-logins";
    public KeepassXcGetLogins()
        : base(ActionName)
    {
    }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("keys")]
    public KeepassXcAssociateKey[] Keys { get; set; }
}