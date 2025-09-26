using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record MangaDexChapter(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("attributes")] ChapterAttributes Attributes,
            [property: JsonPropertyName("relationships")] Relationship[] Relationships
        );
}
