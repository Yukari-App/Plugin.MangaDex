using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record Tag(
            [property: JsonPropertyName("attributes")] TagAttributes Attributes
        );
}
