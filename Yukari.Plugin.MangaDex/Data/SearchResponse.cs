using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record SearchResponse(
            [property: JsonPropertyName("data")] MangaDexComic[] Data
        );
}
