# URL Samurai ⚔️

**Minimalistic, privacy-friendly URL shortener. Fast. Free. No tracking.**

> Live demo: [www.sshare.dev](https://www.sshare.dev)  
> Telegram bot: [@UrlShortenerNinjaBot](https://t.me/UrlShortenerNinjaBot)  
> API: [Swagger UI](https://www.twik.cc/swagger/index.html)  
> Alfred Workflow: [Download](https://github.com/denysovkos/shorten-url-alfred-workflow/releases/tag/v1.0)
---

## ✨ Features

- Blazing-fast short link creation
- URL expiration (1 day to 6 months)
- No ads, no tracking (unless registered)
- Redis caching for performance
- API-ready (with Swagger)
- Telegram bot & Alfred integration
- Clean and responsive Blazor UI

---

## 🚀 Getting Started

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- PostgreSQL (or modify for any EF Core-supported DB)
- Redis (for caching)
- Optional: Docker, for running services locally

### Setup

```bash
git clone https://github.com/denysovkos/url-samurai-v2.git
cd url-samurai-v2

# Set up DB & Redis (via docker-compose or manually)
# Apply migrations
dotnet ef database update

# Run the app
dotnet run --project UrlSamurai
```

> Set your connection strings and Redis config in `appsettings.json`.

---

## 🔐 Privacy & Security

- **Tracking**: No user or usage data stored unless registered.
- **Link Validity**: Links expire after selected time (default: 180 days).
- **User Auth**: Optional – logged-in users can manage links (future feature).
- **Malicious URLs**: Currently no reporting system; use responsibly.

---

## 🧩 Extendability

You can easily:

- Add link statistics
- Integrate authentication (e.g., GitHub, Google)
- Support custom aliases
- Enable link preview or QR code generation

> Contributions and PRs are welcome!

---

## 🔄 Folder Structure (Key Parts)

```
UrlSamurai/
├── Components/         # UI components
├── Data/               # EF Core models and DbContext
├── Pages/              # Main Blazor pages (e.g., Index.razor)
├── Services/           # RedisCacheService, etc.
└── wwwroot/            # Static files (CSS, icons)
```

---

## 📜 License & Contribution Policy

- **License**: MIT – feel free to fork, use, or adapt.
- **Copying**: Please keep reference to the original project (`https://github.com/denysovkos/url-samurai-v2`).
- **Contribution**:
  - Open an issue before major changes
  - Follow clean, minimalistic coding style
  - Write concise commit messages

---

## 📫 Contact

Built with ❤️ by [Kostiantyn Denysov](https://www.denysov.me)  
For bugs, suggestions or abuse reports, contact via footer links on [sshare.dev](https://www.sshare.dev)

---

## ⚠️ Disclaimer

This is a personal project provided **as-is**. I take **no responsibility** for the content of shortened URLs, their availability, or third-party misuse. Use at your own risk.
