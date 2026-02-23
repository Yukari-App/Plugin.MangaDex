using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record DetailsResponse([property: JsonPropertyName("data")] MangaDexComic Data);
}
