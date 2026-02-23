using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record TagAttributes(
        [property: JsonPropertyName("name")] Dictionary<string, string> Name
    );
}
