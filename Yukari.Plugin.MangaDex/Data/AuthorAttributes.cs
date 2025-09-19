using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record AuthorAttributes(
            [property: JsonPropertyName("name")] string Name
        );
}
