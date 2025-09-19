using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record AuthorResponse(
            [property: JsonPropertyName("data")] Author Data
        );
}
