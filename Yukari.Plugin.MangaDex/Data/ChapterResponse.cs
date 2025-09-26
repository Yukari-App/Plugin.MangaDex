using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ChapterResponse(
            [property: JsonPropertyName("data")] MangaDexChapter[] Data
        );
}
