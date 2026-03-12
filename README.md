<div align="center">
  <h1>
    <img src="https://mangadex.org/img/brand/mangadex-logo.svg" alt="MangaDex Logo" width="46" valign="middle"/> Yukari.Plugin.MangaDex
  </h1>
</div>

<div align="center">
  
  ![GitHub Repo stars](https://img.shields.io/github/stars/Yukari-App/Plugin.MangaDex?style=for-the-badge&color=FF6740)
  ![GitHub last commit](https://img.shields.io/github/last-commit/Yukari-App/Plugin.MangaDex?style=for-the-badge&color=E6E6E6)
  ![GitHub repo size](https://img.shields.io/github/repo-size/Yukari-App/Plugin.MangaDex?style=for-the-badge&color=FF6740)
</div>

<div align="center">
    <h2>📖 Overview</h2>

**Yukari.Plugin.MangaDex** is a community plugin implementing the `IComicSource` interface from **[Yukari.Core](https://github.com/Yukari-App/Core)**.

It connects to the **[MangaDex](https://mangadex.org/)** official API with full filter support (age rating, status, demographic, tags, genres...).

Features multiple languages (English, Português-BR, Español, Français, etc.) with localized titles/descriptions where available.

Built for the **[Yukari](https://github.com/Yukari-App/Yukari)** Windows reader app.
</div>

<div align="center">
    <h2>📚 Comic Source Installation</h2>

**Yukari** doesn't come with pre-installed **Comic Sources** for legal reasons. You add the sources you want through **community plugins**.
</div>

- Go to **Releases** and download the `.dll` file from the latest version;
    - If your installed **Yukari** is not up to date, download a **compatible** plugin.
- Inside **Yukari**, go to **Settings** and look for **Sources**;
- Click on **Add New Source** and select the downloaded `.dll` Plugin;
- Done. Now you can go to **Discover** and search for **Comics** in that source.

<div align="center">
    <h2>🗒️ Notes</h2>
</div>

- **Rate limits**: MangaDex enforces strict limits. The plugin handles 429 errors gracefully.
- No login required — uses public API endpoints.
- **Filters**: Supports content rating, status, demographic, tags & genres (UUID-based).
- **Languages**: 20+ available; chapters filtered by user-selected language.
- **Performance**: Lazy static filters/languages, shared `HttpClient` with custom User-Agent.
- **Errors**: Returns empty results instead of crashing on API issues (e.g., invalid ID).

<div align="center">
    <h2>🤝 Contributing</h2>
  
Contributions are welcome! You can help improve **Yukari.Plugin.MangaDex** in several ways:
</div>

- 🐛 **Report issues**: Found a bug or unexpected behavior? Open an [issue](../../issues) describing the problem.
- ✨ **Suggest features**: Have an idea to make **Yukari.Plugin.MangaDex** better? Share it in the issues tab.
- 🔧 **Submit pull requests**: Fix bugs, improve code quality, or add new features.

<div align="center">
  <h2>📜 License</h2>

This project is licensed under the **GPL-3.0**. See the [LICENSE](LICENSE) file for details.
</div>