using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record Cover(
            [property: JsonPropertyName("attributes")] CoverAttributes Attributes
        );
}
