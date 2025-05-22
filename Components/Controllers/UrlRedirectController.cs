using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Components.Services;
using UrlSamurai.Data;
using UrlSamurai.Data.Entities;
using System.Net;

namespace UrlSamurai.Components.Controllers;

[ApiController]
[Route("u")]
public class UrlRedirectController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("{shortId}")]
    public async Task<IActionResult> RedirectToOriginal(string shortId)
    {
        if (string.IsNullOrWhiteSpace(shortId))
            return BadRequest("Missing short ID.");

        var urlEntry = await db.Urls.FirstOrDefaultAsync(u => u.ShortId == shortId);
        if (urlEntry == null)
            return NotFound("This URL not found.");

        var ip = GetClientIp(HttpContext);
        var country = GeoIpService.GetCountry(ip);

        var visit = new UrlVisit
        {
            ShortId = shortId,
            Country = country
        };

        db.UrlVisit.Add(visit);
        await db.SaveChangesAsync();

        return Redirect(urlEntry.UrlValue);
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