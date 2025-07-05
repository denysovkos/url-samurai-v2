using System.Globalization;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Data;
using UrlSamurai.Data.Entities;

namespace UrlSamurai.Components.Services;

public interface IUrlsService
{
    Task<Data.Entities.Urls?> FindUrl(string shortId);

    Task SaveStatistics(string shortId, string? ip);
    
    Task<(string shortId, string irlValue)> SaveUrl(string url, string? ownerId, string? source = null);
}

public class UrlsService (ApplicationDbContext db) : IUrlsService
{
    public async Task<Data.Entities.Urls?> FindUrl(string shortId)
    {
        var urlEntry = await db.Urls.FirstOrDefaultAsync(u => u.ShortId == shortId);
        Console.WriteLine($"Expected time {urlEntry?.ValidTill} current time {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
        return urlEntry?.ValidTill >= DateTime.UtcNow ? urlEntry : null;
    }
    
    public async Task SaveStatistics(string shortId, string? ip)
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

    public async Task<(string shortId, string irlValue)> SaveUrl(string url, string? ownerId, string? source = null)
    {
        var newUrl = new Data.Entities.Urls
        {
            UrlValue = url,
            CreatedAt = DateTime.UtcNow,
            OwnerId = ownerId,
            ValidTill = DateTime.UtcNow.AddDays(180),
        };
        
        Console.WriteLine($"Saving URL: {newUrl.UrlValue} owner: {newUrl.OwnerId} source: ${source}");

        await db.Urls.AddAsync(newUrl);
        await db.SaveChangesAsync();
        
        return (newUrl.ShortId!, newUrl.UrlValue);
    }
    
}
