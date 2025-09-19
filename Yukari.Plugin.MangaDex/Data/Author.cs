using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record Author(
            [property: JsonPropertyName("attributes")] AuthorAttributes Attributes
        );
}
