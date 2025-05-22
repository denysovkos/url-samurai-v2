using System.Net;
using MaxMind.GeoIP2;

namespace UrlSamurai.Components.Services;

public static class GeoIpService
{
    private static readonly string DbPath = Path.Combine(
        Path.GetDirectoryName(typeof(GeoIpService).Assembly.Location)!,
        "GeoLite2-Country.mmdb"
    );

    public static string? GetCountry(string? ip)
    {
        Console.WriteLine($"Redirect requested URL: {ip}");
        if (string.IsNullOrWhiteSpace(ip))
            return null;

        try
        {
            var parsedIp = System.Net.IPAddress.Parse(ip);
            if (!IsPublic(parsedIp))
                return null;

            using var reader = new DatabaseReader(DbPath);
            var country = reader.Country(parsedIp);
            return country?.Country?.IsoCode;
        }
        catch
        {
            return null;
        }
    }

    
    private static bool IsPublic(IPAddress ip)
    {
        return !(IPAddress.IsLoopback(ip)
                 || ip.IsIPv6LinkLocal
                 || ip.IsIPv6SiteLocal
                 || ip.IsIPv6Multicast
                 || ip.ToString().StartsWith("10.")
                 || ip.ToString().StartsWith("192.168.")
                 || ip.ToString().StartsWith("172.16.")
                 || ip.ToString().StartsWith("172.17.")
                 || ip.ToString().StartsWith("172.18.")
                 || ip.ToString().StartsWith("172.19.")
                 || ip.ToString().StartsWith("172.20.")
                 || ip.ToString().StartsWith("172.21.")
                 || ip.ToString().StartsWith("172.22.")
                 || ip.ToString().StartsWith("172.23.")
                 || ip.ToString().StartsWith("172.24.")
                 || ip.ToString().StartsWith("172.25.")
                 || ip.ToString().StartsWith("172.26.")
                 || ip.ToString().StartsWith("172.27.")
                 || ip.ToString().StartsWith("172.28.")
                 || ip.ToString().StartsWith("172.29.")
                 || ip.ToString().StartsWith("172.30.")
                 || ip.ToString().StartsWith("172.31.")
                 || ip.ToString().StartsWith("100.64.")
                 || ip.ToString().StartsWith("127.")
            );
    }

}