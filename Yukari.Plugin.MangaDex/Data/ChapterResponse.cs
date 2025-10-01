using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ChapterResponse(
            [property: JsonPropertyName("data")] MangaDexChapter[] Data,
            [property: JsonPropertyName("offset")] int Offset,
            [property: JsonPropertyName("total")] int Total
        );
}
