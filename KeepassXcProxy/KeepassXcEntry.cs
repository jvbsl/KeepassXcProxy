using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcEntry
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("group")]
    public string Group { get; set; }

    [JsonPropertyName("totp")]
    public string? Totp { get; set; }

    [JsonPropertyName("expired")]
    public bool? Expired { get; set; }
    
    [JsonPropertyName("skipAutoSubmit")]
    public bool? SkipAutoSubmit { get; set; }
    
    [JsonPropertyName("stringFields")]
    public KeyValuePair<string, string>[]? StringFields { get; set; }
}