using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using UrlSamurai.Components.Cache;
using UrlSamurai.Data;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("url")]
public class UrlSaveController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, RedisCacheService redis) : ControllerBase
{
    public class UrlInput
    {
        public string Url { get; set; } = default!;
    }

    [HttpPost]
    public async Task<IActionResult> SaveUrl([FromBody] UrlInput input, [FromQuery] string? source)
    {
        if (string.IsNullOrWhiteSpace(input.Url) || !UrlValidator.IsValid(input.Url))
        {
            return BadRequest("URL is required.");
        }

        var user = httpContextAccessor.HttpContext?.User;
        var ownerId = user?.Identity?.IsAuthenticated == true
            ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;

        var newUrl = new Data.Entities.Urls
        {
            UrlValue = input.Url,
            CreatedAt = DateTime.UtcNow,
            OwnerId = ownerId,
        };
        
        Console.WriteLine($"Saving URL: {newUrl.UrlValue} owner: {newUrl.OwnerId} source: ${source}");

        await db.Urls.AddAsync(newUrl);
        await db.SaveChangesAsync();

        await redis.SetAsync(newUrl.ShortId!, newUrl.UrlValue);
        
        var shortLink = $"https://www.twik.cc/u/{newUrl.ShortId}";

        return Ok(new { id = newUrl.ShortId, url = shortLink });
    }
    
    internal static class UrlValidator
    {
        private static readonly Regex UrlRegex = new(@"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$", RegexOptions.Compiled);

        public static bool IsValid(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) && UrlRegex.IsMatch(url);
        }
    }
}