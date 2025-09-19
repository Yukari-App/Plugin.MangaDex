using System.Net.Http.Json;
﻿using Yukari.Core.Models;
using Yukari.Core.Sources;
using Yukari.Plugin.MangaDex.Data;

namespace Yukari.Plugin.MangaDex
{
    public class MangaDexSource : IMangaSource
    {
        public string Name => "MangaDex";

        public Task<List<Comic>> SearchAsync(string query) => throw new NotImplementedException();
        public Task<List<Comic>> GetTrendingAsync() => throw new NotImplementedException();
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
    }
}
