using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Components.Services;
using UrlSamurai.Data;
using UrlSamurai.Data.Entities;

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

        var country = HttpContext.Connection.RemoteIpAddress?.ToString();
        var visit = new UrlVisit
        {
            ShortId = shortId,
            Country = GeoIpService.GetCountry(country)
        };

        db.UrlVisit.Add(visit);
        await db.SaveChangesAsync();

        return Redirect(urlEntry.UrlValue);
    }

}