using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlSamurai.Components.Cache;
using UrlSamurai.Components.Controllers.UrlControllerBase;
using UrlSamurai.Components.Services;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("url")]
public class UrlSaveController(IUrlsService urlService, IHttpContextAccessor httpContextAccessor, IRedisCacheService redis) : UrlsControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveUrl([FromBody] Dto.UrlInput input, [FromQuery] string? source)
    {
        if (IsUrlValid(input.Url))
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

        return Ok(new { id = shortId, url = shortLink });
    }
    
    // Alfred redirects AAAAAAAAAA!!!!
    // Nothing to do with this :(
    [HttpGet("{shortId}")]
    public async Task<IActionResult> RedirectToOriginal(string shortId)
    {
        if (string.IsNullOrWhiteSpace(shortId))
        {
            return BadRequest("Missing short ID.");
        }
        
        var ip = GetClientIp(HttpContext);

        var cached = await redis.GetAsync(shortId);
        if (cached != null)
        {
            await urlService.SaveStatistics(shortId, ip);
            return Redirect(cached);
        }

        var urlEntry = await urlService.FindUrl(shortId);
        if (urlEntry == null)
        {
            return NotFound("This URL not found.");
        }


        await urlService.SaveStatistics(shortId, ip);

        return Redirect(urlEntry.UrlValue);
    }
}