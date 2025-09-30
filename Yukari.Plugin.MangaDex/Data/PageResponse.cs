using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record PageResponse(
            [property: JsonPropertyName("baseUrl")] string BaseUrl,
            [property: JsonPropertyName("chapter")] ChapterPages ChapterPages
        );
}
