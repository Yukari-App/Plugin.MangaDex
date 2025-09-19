using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record MangaDexComic(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("attributes")] ComicAttributes Attributes,
            [property: JsonPropertyName("relationships")] relationship[] Relationships
        );
}
