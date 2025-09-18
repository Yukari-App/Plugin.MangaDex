using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record relationship(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("type")] string Type
        );
}
