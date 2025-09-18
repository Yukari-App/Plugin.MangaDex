using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record CoverResponse(
            [property: JsonPropertyName("data")] Cover Data
        );
}
