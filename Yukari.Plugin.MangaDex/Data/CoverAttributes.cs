using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record CoverAttributes(
            [property: JsonPropertyName("fileName")] string FileName
        );
}
