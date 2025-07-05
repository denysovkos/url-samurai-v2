using Microsoft.AspNetCore.Mvc;
using UrlSamurai.Components.Services;
using UrlSamurai.Components.Cache;
using UrlSamurai.Components.Controllers.UrlControllerBase;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("u")]
public class UrlRedirectController(IUrlsService urlService, IRedisCacheService redis) : UrlsControllerBase
{
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