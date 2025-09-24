using System.Net.Http.Json;
using Yukari.Core.Models;
using Yukari.Core.Sources;
using Yukari.Plugin.MangaDex.Data;

namespace Yukari.Plugin.MangaDex
{
    public class MangaDexSource : IMangaSource
    {
        public string Name => "MangaDex";

        private const string BaseUrl = "https://api.mangadex.org";

        private readonly HttpClient _httpClient = new HttpClient();

        public MangaDexSource()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Yukari.Plugin.MangaDex");
        }

        public async Task<List<Comic>> SearchAsync(string query)
        {
            string searchUrl = $"{BaseUrl}/manga?limit=24&title={query}";

            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            MangaDexComic[] searchResults = (await response.Content.ReadFromJsonAsync<SearchResponse>())?.Data;

            var tasks = searchResults.Select(async result =>
            {
                var coverUrl = result.Relationships.FirstOrDefault(r => r.Type == "cover_art")?.Id is { } coverId ?
                    await GetCoverUrl(result.Id, coverId) : null;

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
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public async Task<List<Comic>> GetTrendingAsync()
        {
            string trendingUrl = $"{BaseUrl}/manga?limit=24&order[followedCount]=desc";

            var response = await _httpClient.GetAsync(trendingUrl);
            response.EnsureSuccessStatusCode();

            MangaDexComic[] trendingResults = (await response.Content.ReadFromJsonAsync<SearchResponse>())?.Data;

            var tasks = trendingResults.Select(async result =>
            {
                var coverUrl = result.Relationships.FirstOrDefault(r => r.Type == "cover_art")?.Id is { } coverId ?
                    await GetCoverUrl(result.Id, coverId) : null;

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
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public Task<Comic?> GetDetailsAsync(string mangaId) => throw new NotImplementedException();
        public Task<List<ChapterPage>> GetChapterPagesAsync(string chapterId) => throw new NotImplementedException();
        public Task<List<Chapter>> GetAllChaptersAsync(string mangaId, string language) => throw new NotImplementedException();

        public async Task<string> GetAuthorName(string authorId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/author/{authorId}");
            response.EnsureSuccessStatusCode();

            Author authorData = (await response.Content.ReadFromJsonAsync<AuthorResponse>())?.Data;

            return authorData.Attributes.Name;
        }

        public async Task<string> GetCoverUrl(string mangaId, string coverId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/cover/{coverId}");
            response.EnsureSuccessStatusCode();

            Cover coverData = (await response.Content.ReadFromJsonAsync<CoverResponse>())?.Data;

            return $"https://uploads.mangadex.org/covers/{mangaId}/{coverData.Attributes.FileName}";
        }

        private static string GetLocalized(Dictionary<string, string> dict, string fallback = "Unknown") =>
            dict.TryGetValue("en", out var value) ? value : dict.Values.FirstOrDefault() ?? fallback;
    }
}
