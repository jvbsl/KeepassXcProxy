using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public enum NativeMessagingType
{
    [EnumMember(Value = "stdio")]
    Stdio
}

public class NativeMessagingConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("path")]
    public string Path { get; set; }
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NativeMessagingType Type { get; set; }
    
    [JsonPropertyName("allowed_origins")]
    public string[] AllowedOrigins { get; set; }
}