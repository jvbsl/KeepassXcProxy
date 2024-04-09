using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcErrorResponse : KeepassXcBaseResponse
{
    [JsonPropertyName("name")]
    public string Action { get; set; }
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }
    [JsonPropertyName("error")]
    public string Error { get; set; }
}