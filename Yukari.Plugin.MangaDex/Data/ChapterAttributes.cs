using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ChapterAttributes(
            [property: JsonPropertyName("volume")] string? Volume,
            [property: JsonPropertyName("chapter")] string? Chapter,
            [property: JsonPropertyName("title")] string? Title,
            [property: JsonPropertyName("translatedLanguage")] string? Language,
            [property: JsonPropertyName("updatedAt")] DateTimeOffset UpdatedAt,
            [property: JsonPropertyName("pages")] int Pages,
            [property: JsonPropertyName("relationships")] Relationship[] Relationships
        );
}
