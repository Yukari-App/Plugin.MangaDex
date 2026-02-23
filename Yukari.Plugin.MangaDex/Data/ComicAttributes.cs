using System.Formats.Asn1;
using System.Text.Json.Serialization;

namespace Yukari.Plugin.MangaDex.Data
{
    internal record ComicAttributes(
        [property: JsonPropertyName("title")] Dictionary<string, string> Title,
        [property: JsonPropertyName("altTitles")] List<Dictionary<string, string>> AltTitles,
        [property: JsonPropertyName("description")] Dictionary<string, string> Description,
        [property: JsonPropertyName("year")] int? Year,
        [property: JsonPropertyName("tags")] Tag[] Tags,
        [property: JsonPropertyName("availableTranslatedLanguages")] string[] Languages
    );
}
