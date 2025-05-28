using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Components.Services;
using UrlSamurai.Data;
using UrlSamurai.Data.Entities;
using System.Net;
using UrlSamurai.Components.Cache;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("u")]
public class UrlRedirectController(ApplicationDbContext db, RedisCacheService redis) : ControllerBase
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
            await SaveStatistics(shortId, ip);
            return Redirect(cached);
        }

        var urlEntry = await UrlsService.FindUrl(db, shortId);
        if (urlEntry == null)
        {
            return NotFound("This URL not found.");
        }


        await SaveStatistics(shortId, ip);

        return Redirect(urlEntry.UrlValue);
    }

    private async Task SaveStatistics(string shortId, string? ip)
    {
        if (string.IsNullOrWhiteSpace(shortId))
        {
            return;
        }
        
        var country = GeoIpService.GetCountry(ip);
        
        var visit = new UrlVisit
        {
            ShortId = shortId,
            Country = country
        };

        db.UrlVisit.Add(visit);
        await db.SaveChangesAsync();
    }

    private static string? GetClientIp(HttpContext context)
    {
        var headers = context.Request.Headers;
        string? ip = headers["X-Forwarded-For"].FirstOrDefault()
                     ?? headers["X-Real-IP"].FirstOrDefault()
                     ?? context.Connection.RemoteIpAddress?.ToString();

        if (IPAddress.TryParse(ip, out var parsed) && parsed.IsIPv4MappedToIPv6)
            ip = parsed.MapToIPv4().ToString();

        return ip;
    }
}