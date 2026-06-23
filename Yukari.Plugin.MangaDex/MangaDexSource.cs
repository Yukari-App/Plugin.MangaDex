using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Yukari.Core.Models;
using Yukari.Core.Sources;
using Yukari.Plugin.MangaDex.Data;

namespace Yukari.Plugin.MangaDex;

[ComicSourceMetadata(
    "MangaDex",
    "2.3.0+core2.3.0",
    "https://github.com/Yukari-App/Plugin.MangaDex/releases",
    "https://mangadex.org/img/brand/mangadex-logo.svg",
    "A community-driven manga database and reader."
)]
public class MangaDexSource : IComicSource
{
    private const int DefaultPageSize = 24;

    private static IReadOnlyList<Filter>? _filters;
    private static IReadOnlyDictionary<string, string>? _languages;

    public IReadOnlyList<Filter> Filters =>
        _filters ??= [
            new Filter(
                Key: "contentRating[]",
                DisplayName: "Age Rating",
                Options:
                [
                    new FilterOption("safe", "Safe", true),
                    new FilterOption("suggestive", "Suggestive", true),
                    new FilterOption("erotica", "Erotica", true),
                    new FilterOption("pornographic", "Pornographic", false),
                ],
                AllowMultiple: true
            ),
            new Filter(
                Key: "status[]",
                DisplayName: "Status",
                Options:
                [
                    new FilterOption("ongoing", "Ongoing", false),
                    new FilterOption("completed", "Completed", false),
                    new FilterOption("hiatus", "Paused", false),
                    new FilterOption("cancelled", "Cancelled", false),
                ],
                AllowMultiple: true
            ),
            new Filter(
                Key: "publicationDemographic[]",
                DisplayName: "Demographic",
                Options:
                [
                    new FilterOption("shounen", "Shounen", false),
                    new FilterOption("shoujo", "Shoujo", false),
                    new FilterOption("josei", "Josei", false),
                    new FilterOption("seinen", "Seinen", false),
                ],
                AllowMultiple: true
            ),
            new Filter(
                Key: "tags",
                DisplayName: "Tags",
                Options:
                [
                    new FilterOption("b11fda93-8f1d-4bef-b2ed-8803d3733170", "4-Koma", false),
                    new FilterOption("f4122d1c-3b44-44d0-9936-ff7502c39ad3", "Adaptation", false),
                    new FilterOption("51d83883-4103-437c-b4b1-731cb73d786c", "Anthology", false),
                    new FilterOption("0a39b5a1-b235-4886-a747-1d05d216532d", "Award Winning", false),
                    new FilterOption("b13b2a48-c720-44a9-9c77-39c9979373fb", "Doujinshi", false),
                    new FilterOption("7b2ce280-79ef-4c09-9b58-12b7c23a9b78", "Fan Colored", false),
                    new FilterOption("f5ba408b-0e7a-484d-8d49-4e9125ac96de", "Full Color", false),
                    new FilterOption("3e2b8dae-350e-4ab8-a8ce-016e844b9f0d", "Long Strip", false),
                    new FilterOption("320831a8-4026-470b-94f6-8353740e6f04", "Official Colored", false),
                    new FilterOption("0234a31e-a729-4e28-9d6a-3f87c4966b9e", "Oneshot", false),
                    new FilterOption("891cf039-b895-47f0-9229-bef4c96eccd4", "Self-Published", false),
                    new FilterOption("e197df38-d0e7-43b5-9b09-2842d0c326dd", "Web Comic", false),
                ],
                AllowMultiple: true
            ),
            new Filter(
                Key: "genres",
                DisplayName: "Genres",
                Options:
                [
                    new FilterOption("391b0423-d847-456f-aff0-8b0cfc03066b", "Action", false),
                    new FilterOption("87cc87cd-a395-47af-b27a-93258283bbc6", "Adventure", false),
                    new FilterOption("5920b825-4181-4a17-beeb-9918b0ff7a30", "Boys Love", false),
                    new FilterOption("4d32cc48-9f00-4cca-9b5a-a839f0764984", "Comedy", false),
                    new FilterOption("5ca48985-9a9d-4bd8-be29-80dc0303db72", "Crime", false),
                    new FilterOption("b9af3a63-f058-46de-a9a0-e0c13906197a", "Drama", false),
                    new FilterOption("cdc58593-87dd-415e-bbc0-2ec27bf404cc", "Fantasy", false),
                    new FilterOption("a3c67850-4684-404e-9b7f-c69850ee5da6", "Girls Love", false),
                    new FilterOption("33771934-028e-4cb3-8744-691e866a923e", "Historical", false),
                    new FilterOption("cdad7e68-1419-41dd-bdce-27753074a640", "Horror", false),
                    new FilterOption("ace04997-f6bd-436e-b261-779182193d3d", "Isekai", false),
                    new FilterOption("81c836c9-914a-4eca-981a-560dad663e73", "Magical Girls", false),
                    new FilterOption("50880a9d-5440-4732-9afb-8f457127e836", "Mecha", false),
                    new FilterOption("c8cbe35b-1b2b-4a3f-9c37-db84c4514856", "Medical", false),
                    new FilterOption("ee968100-4191-4968-93d3-f82d72be7e46", "Mystery", false),
                    new FilterOption("b1e97889-25b4-4258-b28b-cd7f4d28ea9b", "Philosophical", false),
                    new FilterOption("423e2eae-a7a2-4a8b-ac03-a8351462d71d", "Romance", false),
                    new FilterOption("256c8bd9-4904-4360-bf4f-508a76d67183", "Sci-Fi", false),
                    new FilterOption("e5301a23-ebd9-49dd-a0cb-2add944c7fe9", "Slice of Life", false),
                    new FilterOption("69964a64-2f90-4d33-beeb-f3ed2875eb4c", "Sports", false),
                    new FilterOption("7064a261-a137-4d3a-8848-2d385de3a99c", "Superhero", false),
                    new FilterOption("07251805-a27e-4d59-b488-f0bfbec15168", "Thriller", false),
                    new FilterOption("f8f62932-27da-4fe4-8ee1-6779a8c5edba", "Tragedy", false),
                    new FilterOption("acc803a4-c95a-4c22-86fc-eb6b582d82a2", "Wuxia", false),
                ],
                AllowMultiple: true
            ),
        ];

    public IReadOnlyDictionary<string, string> Languages =>
        _languages ??= new Dictionary<string, string>
        {
            { "en", "English" },
            { "pt-br", "Português" },
            { "es", "Español" },
            { "es-la", "Español LA" },
            { "fr", "Français" },
            { "de", "Deutsch" },
            { "it", "Italiano" },
            { "ru", "Русский" },
            { "pl", "Polski" },
            { "id", "Bahasa Indonesia" },
            { "vi", "Tiếng Việt" },
            { "ar", "العربية" },
            { "ja", "日本語" },
            { "ja-ro", "日本語‑ro" },
            { "ko", "한국어" },
            { "ko-ro", "한국어‑ro" },
            { "zh", "中文" },
            { "zh-hk", "中文 HK" },
            { "zh-ro", "中文‑ro" },
            { "th", "ไทย" },
            { "tr", "Türkçe" },
            { "ms", "Bahasa Melayu" },
            { "hi", "हिन्दी" },
            { "ca", "Català" },
            { "nl", "Nederlands" },
            { "sv", "Svenska" },
            { "fi", "Suomi" },
        };

    private const string BaseUrl = "https://api.mangadex.org";

    private static readonly HttpClient _httpClient = new HttpClient();

    static MangaDexSource()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Yukari.Plugin.MangaDex/2.3.0");
    }

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
                    Slug: null,
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
            Slug: null,
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

        string[] data = pageResponse.ChapterPages.Data;
        string baseUrl = pageResponse.BaseUrl;
        string hash = pageResponse.ChapterPages.Hash;

        var pages = new List<ChapterPage>(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            pages.Add(new ChapterPage(Number: i + 1, ImageUrl: $"{baseUrl}/data/{hash}/{data[i]}"));
        }
        return pages;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static async Task<T?> GetFromApiAsync<T>(string url, CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync(url, ct);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException(
                "MangaDex Rate Limit Exceeded. Try again later.",
                null,
                HttpStatusCode.TooManyRequests
            );

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(ct);
    }

    private static string? GetNameFromAttributes(object attributes)
    {
        if (attributes is JsonElement element && element.TryGetProperty("name", out var nameProp))
            return nameProp.GetString();
        return null;
    }

    private static string? GetCoverUrl(string comicId, object coverAttributes)
    {
        if (coverAttributes is JsonElement element)
        {
            var fileName = element.GetProperty("fileName").GetString();
            return $"https://uploads.mangadex.org/covers/{comicId}/{fileName}.256.jpg";
        }

        return null;
    }

    private static ComicStatus GetComicStatus(string status) =>
        status switch
        {
            "ongoing" => ComicStatus.Ongoing,
            "completed" => ComicStatus.Completed,
            "hiatus" => ComicStatus.Hiatus,
            "cancelled" => ComicStatus.Cancelled,
            _ => ComicStatus.Unknown,
        };

    private static string? GetLocalizedOrNull(Dictionary<string, string> dict) =>
        dict.TryGetValue("en", out var value) ? value : dict.Values.FirstOrDefault();

    private static string GetLocalized(Dictionary<string, string> dict) =>
        GetLocalizedOrNull(dict) ?? "Unknown";

    private static string ToQueryString(Dictionary<string, string[]> source) =>
        string.Join(
            "&",
            source.SelectMany(kvp =>
                kvp.Value.Select(v => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(v)}")
            )
        );
}
