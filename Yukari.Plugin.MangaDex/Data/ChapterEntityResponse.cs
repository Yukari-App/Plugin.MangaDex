using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ChapterEntityResponse(
        [property: JsonPropertyName("data")] MangaDexChapter Data
    );
}
