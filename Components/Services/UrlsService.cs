using System.Globalization;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Data;

namespace UrlSamurai.Components.Services;

public static class UrlsService
{
    public static async Task<Data.Entities.Urls?> FindUrl(ApplicationDbContext db, string shortId)
    {
        var urlEntry = await db.Urls.FirstOrDefaultAsync(u => u.ShortId == shortId);
        Console.WriteLine($"Expected time {urlEntry?.ValidTill} current time {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
        return urlEntry?.ValidTill >= DateTime.UtcNow ? urlEntry : null;
    }
}
