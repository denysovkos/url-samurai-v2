using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using UrlSamurai.Components.Bot;
using UrlSamurai.Components.Cache;
using UrlSamurai.Components.Controllers.UrlControllerBase;
using UrlSamurai.Components.Services;
using UrlSamurai.Data;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("api/url")]
public class AlfredSaveUrlController(IUrlsService urlService, IHttpContextAccessor httpContextAccessor, IRedisCacheService redis) : UrlsControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveUrl([FromBody] Dto.UrlInput input, [FromQuery] string? source)
    {
        if (string.IsNullOrWhiteSpace(input.Url) || !TelegramInlineBot.UrlValidator.IsValid(input.Url))
        {
            return BadRequest("URL is required.");
        }

        var user = httpContextAccessor.HttpContext?.User;
        var ownerId = user?.Identity?.IsAuthenticated == true
            ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;
        
        var (shortId, urlValue) = await urlService.SaveUrl(input.Url, ownerId, source);

        await redis.SetAsync(shortId!, urlValue);
        
        var shortLink = $"https://www.twik.cc/u/{shortId}";

        return Ok(new { id = shortId, url = shortLink, shortUrlId = shortId });
    }
}