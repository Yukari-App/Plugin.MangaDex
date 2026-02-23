using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ChapterPages(
        [property: JsonPropertyName("hash")] string Hash,
        [property: JsonPropertyName("data")] string[] Data,
        [property: JsonPropertyName("dataSaver")] string[] DataSaver
    );
}
