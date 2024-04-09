using System.Text.Json.Serialization;

namespace KeepassXcProxy;

public class KeepassXcGetDatabaseEntriesResponse : KeepassXcActionResponse
{
    [JsonPropertyName("entries")]
    public KeepassXcSearchEntry[] Entries { get; set; }
}