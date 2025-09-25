using System.Net.Http.Json;
using System.Text.Json;
using Yukari.Core.Models;
using Yukari.Core.Sources;
using Yukari.Plugin.MangaDex.Data;

namespace Yukari.Plugin.MangaDex
{
    public class MangaDexSource : IMangaSource
    {
        public string Name => "MangaDex";
        public string? LogoUrl => "https://mangadex.org/img/brand/mangadex-logo.svg";
        public string? Description => "A community-driven manga database and reader.";

        private const string BaseUrl = "https://api.mangadex.org";

        private readonly HttpClient _httpClient = new HttpClient();

        public MangaDexSource()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Yukari.Plugin.MangaDex");
        }

        public async Task<List<Comic>> SearchAsync(string query)
        {
            string searchUrl = $"{BaseUrl}/manga?limit=24&&includes[]=cover_art&title={query}";

            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            MangaDexComic[] searchResults = (await response.Content.ReadFromJsonAsync<SearchResponse>())?.Data;

            var comics = searchResults.Select(result =>
            {
                var coverUrl = result.Relationships
                    .FirstOrDefault(r => r.Type == "cover_art")?.Attributes is { } coverAttributes
                    ? GetCoverUrl(result.Id, coverAttributes) : null;

                return new Comic(
                    Id: result.Id,
                    Source: Name,
                    Slug: result.Id,
                    Title: GetLocalized(result.Attributes.Title),
                    Author: null,
                    Description: null,
                    Tags: [],
                    Year: null,
                    CoverImageUrl: coverUrl,
                    Langs: []
                );
            }).ToList();

            return comics;
        }

        public async Task<List<Comic>> GetTrendingAsync()
        {
            string trendingUrl = $"{BaseUrl}/manga?limit=24&includes[]=cover_art&order[followedCount]=desc";

            var response = await _httpClient.GetAsync(trendingUrl);
            response.EnsureSuccessStatusCode();

            MangaDexComic[] trendingResults = (await response.Content.ReadFromJsonAsync<SearchResponse>())?.Data;

            var comics = trendingResults.Select(result =>
            {
                var coverUrl = result.Relationships.FirstOrDefault(r => r.Type == "cover_art")?.Attributes is { } coverAttributes
                    ? GetCoverUrl(result.Id, coverAttributes) : null;

                return new Comic(
                    Id: result.Id,
                    Source: Name,
                    Slug: result.Id,
                    Title: GetLocalized(result.Attributes.Title),
                    Author: null,
                    Description: null,
                    Tags: [],
                    Year: null,
                    CoverImageUrl: coverUrl,
                    Langs: []
                );
            }).ToList();

            return comics;
        }

        public Task<Comic?> GetDetailsAsync(string mangaId) => throw new NotImplementedException();
        public Task<List<ChapterPage>> GetChapterPagesAsync(string chapterId) => throw new NotImplementedException();
        public Task<List<Chapter>> GetAllChaptersAsync(string mangaId, string language) => throw new NotImplementedException();

        public string GetAuthorName(object authorAttributes)
        {
            if (authorAttributes is JsonElement element)
            {
                return element.GetProperty("name").GetString();
            }

            throw new ArgumentException("Invalid attributes type");
        }

        public string GetCoverUrl(string mangaId, object coverAttributes)
        {
            if (coverAttributes is JsonElement element)
        {
                var fileName = element.GetProperty("fileName").GetString();
                return $"https://uploads.mangadex.org/covers/{mangaId}/{fileName}";
            }

            throw new ArgumentException("Invalid attributes type");
        }

        private static string GetLocalized(Dictionary<string, string> dict, string fallback = "Unknown") =>
            dict.TryGetValue("en", out var value) ? value : dict.Values.FirstOrDefault() ?? fallback;
    }
}
