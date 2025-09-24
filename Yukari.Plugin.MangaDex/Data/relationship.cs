using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record Relationship(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("attributes")] object? Attributes = null
        );
}
