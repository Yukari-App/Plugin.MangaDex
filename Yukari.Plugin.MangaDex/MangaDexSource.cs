using System.Net;
using Yukari.Core.Models;
using Yukari.Core.Models.Settings;
using Yukari.Core.Sources;
using Yukari.Core.Sources.Http;
using Yukari.Plugin.MangaDex.Data;

namespace Yukari.Plugin.MangaDex;

[ComicSourceMetadata(
    "MangaDex",
    "2.5.0+core2.5.0",
    "https://github.com/Yukari-App/Plugin.MangaDex/releases",
    "https://mangadex.org/img/brand/mangadex-logo.svg",
    "A community-driven manga database and reader."
)]
public partial class MangaDexSource : IComicSource, IRequiresHttpClient, IConfigurableSource
{
    private const int DefaultPageSize = 24;
    private const string BaseUrl = "https://api.mangadex.org";

    private ISharedHttpClient? _httpClient;
    private bool _useDataSaver;

    public IReadOnlyList<Filter> Filters => _filters;
    public IReadOnlyDictionary<string, string> Languages => _languages;
    public IReadOnlyList<SourceSetting> Settings => _settings;

    public void SetHttpClient(ISharedHttpClient sharedHttpClient) => _httpClient = sharedHttpClient;

    public void ApplySettings(ISourceSettings settings) => _useDataSaver = settings.GetBool("data_saver");

    public async Task<IReadOnlyList<Comic>> SearchAsync(
        string query,
        IReadOnlyDictionary<string, IReadOnlyList<string>> filters,
        int page = 1,
        CancellationToken ct = default
    )
    {
        var queryParams = new Dictionary<string, string[]>
        {
            ["limit"] = [DefaultPageSize.ToString()],
            ["offset"] = [((page - 1) * DefaultPageSize).ToString()],
            ["includes[]"] = ["cover_art"],
            ["title"] = [query],
        };

        foreach (var kvp in filters)
        {
            string apiKey = kvp.Key switch
            {
                "tags" or "genres" => "includedTags[]",
                _ => kvp.Key,
            };

            queryParams[apiKey] = kvp.Value.ToArray();
        }

        string searchUrl = $"{BaseUrl}/manga?{ToQueryString(queryParams)}";

        MangaDexComic[]? searchResults = (await GetFromApiAsync<SearchResponse>(searchUrl, ct))?.Data;

        if (searchResults is not { Length: > 0 })
            return Array.Empty<Comic>();

        var comics = searchResults
            .Select(result =>
            {
                var coverUrl = result.Relationships.FirstOrDefault(r => r.Type == "cover_art")?.Attributes
                    is { } coverAttributes
                    ? GetCoverUrl(result.Id, coverAttributes)
                    : null;

                return new Comic(
                    Id: result.Id,
                    ComicUrl: null,
                    Title: GetLocalized(result.Attributes.Title),
                    Author: null,
                    Description: null,
                    Tags: [],
                    Year: null,
                    CoverImageUrl: coverUrl,
                    Langs: [],
                    Status: GetComicStatus(result.Attributes.Status)
                );
            })
            .ToList();

        return comics;
    }

    public async Task<IReadOnlyList<Comic>> GetTrendingAsync(
        IReadOnlyDictionary<string, IReadOnlyList<string>> filters,
        int page = 1,
        CancellationToken ct = default
    )
    {
        var trendingFilters = new Dictionary<string, IReadOnlyList<string>>(filters)
        {
            ["order[followedCount]"] = ["desc"],
        };

        return await SearchAsync(string.Empty, trendingFilters, page, ct);
    }

    public async Task<Comic?> GetDetailsAsync(string comicId, CancellationToken ct = default)
    {
        string detailsUrl = $"{BaseUrl}/manga/{comicId}?includes[]=author&includes[]=cover_art";

        MangaDexComic? detailsResult = (await GetFromApiAsync<DetailsResponse>(detailsUrl, ct))?.Data;

        if (detailsResult is null)
            return null;

        var author = detailsResult.Relationships.FirstOrDefault(r => r.Type == "author")?.Attributes
            is { } authorAttributes
            ? GetNameFromAttributes(authorAttributes)
            : null;

        var coverUrl = detailsResult.Relationships.FirstOrDefault(r => r.Type == "cover_art")?.Attributes
            is { } coverAttributes
            ? GetCoverUrl(detailsResult.Id, coverAttributes)
            : null;

        return new Comic(
            Id: detailsResult.Id,
            ComicUrl: $"https://mangadex.org/title/{detailsResult.Id}",
            Title: GetLocalized(detailsResult.Attributes.Title),
            Author: author,
            Description: GetLocalizedOrNull(detailsResult.Attributes.Description),
            Tags: detailsResult.Attributes.Tags.Select(tag => GetLocalized(tag.Attributes.Name)).ToArray(),
            Year: detailsResult.Attributes.Year,
            CoverImageUrl: coverUrl,
            Langs: detailsResult.Attributes.Languages,
            Status: GetComicStatus(detailsResult.Attributes.Status)
        );
    }

    public async Task<IReadOnlyList<Chapter>> GetAllChaptersAsync(
        string comicId,
        string language,
        CancellationToken ct = default
    )
    {
        const int limit = 500;

        List<MangaDexChapter> chapterResults = new();
        int offset = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var queryParams = new Dictionary<string, string[]>
            {
                ["limit"] = [limit.ToString()],
                ["offset"] = [offset.ToString()],
                ["order[chapter]"] = ["asc"],
                ["includeEmptyPages"] = ["0"],
                ["translatedLanguage[]"] = [language],
                ["includes[]"] = ["scanlation_group"],
                ["contentRating[]"] = ["safe", "suggestive", "erotica", "pornographic"],
            };
            string chaptersUrl = $"{BaseUrl}/manga/{comicId}/feed?{ToQueryString(queryParams)}";

            ChaptersCollectionResponse? chaptersResponse = await GetFromApiAsync<ChaptersCollectionResponse>(
                chaptersUrl,
                ct
            );

            if (chaptersResponse == null)
                return new List<Chapter>();

            if (chaptersResponse?.Data is { Length: > 0 } data)
                chapterResults.AddRange(data);
            else
                break;

            if (chapterResults.Count >= chaptersResponse.Total)
                break;

            offset += limit;
            await Task.Delay(200, ct);
        }

        return chapterResults
            .Select(result =>
            {
                var groups = result
                    .Relationships.Where(r => r.Type == "scanlation_group")
                    .Select(r => r.Attributes is { } attrs ? GetNameFromAttributes(attrs) : null)
                    .Where(name => name != null)
                    .Select(name => name!)
                    .ToArray();

                return new Chapter(
                    Id: result.Id,
                    Title: result.Attributes.Title,
                    Number: result.Attributes.Chapter,
                    Volume: result.Attributes.Volume,
                    Language: result.Attributes.Language,
                    Groups: groups,
                    LastUpdate: DateOnly.FromDateTime(result.Attributes.UpdatedAt.DateTime),
                    Pages: result.Attributes.Pages
                );
            })
            .ToList();
    }

    public async Task<IReadOnlyList<ChapterPage>> GetChapterPagesAsync(
        string comicId,
        string chapterId,
        CancellationToken ct = default
    )
    {
        string pagesUrl = $"{BaseUrl}/at-home/server/{chapterId}";

        PageResponse? pageResponse = await GetFromApiAsync<PageResponse>(pagesUrl, ct);

        if (pageResponse is null)
            return new List<ChapterPage>();

        bool useDataSaver = _useDataSaver;
        string[] data = useDataSaver ? pageResponse.ChapterPages.DataSaver : pageResponse.ChapterPages.Data;
        string qualitySegment = useDataSaver ? "data-saver" : "data";
        string baseUrl = pageResponse.BaseUrl;
        string hash = pageResponse.ChapterPages.Hash;

        var pages = new List<ChapterPage>(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            pages.Add(
                new ChapterPage(Number: i + 1, ImageUrl: $"{baseUrl}/{qualitySegment}/{hash}/{data[i]}")
            );
        }
        return pages;
    }

    public async Task<byte[]?> GetImageBytesAsync(string imageUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        using var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
        request.Headers.Add("Referer", "https://mangadex.org/");

        using var response = await _httpClient!.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException(
                "MangaDex Rate Limit Exceeded. Try again later.",
                null,
                HttpStatusCode.TooManyRequests
            );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
