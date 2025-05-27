using System.Net;
using MaxMind.GeoIP2;

namespace UrlSamurai.Components.Services;

public static class GeoIpService
{
    // private static readonly string DbPath = Path.Combine(
    //     Path.GetDirectoryName(typeof(GeoIpService).Assembly.Location)!,
    //     "GeoLite2-Country.mmdb"
    // );

    private const string DbPath = "GeoLite2-Country.mmdb";

    public static string? GetCountry(string? ipList)
    {
        Console.WriteLine($"Redirect requested IP(s): {ipList}");
        if (string.IsNullOrWhiteSpace(ipList))
            return null;

        var ips = ipList.Split(',').Select(ip => ip.Trim());

        foreach (var ip in ips)
        {
            try
            {
                var parsedIp = IPAddress.Parse(ip);
                if (parsedIp.IsIPv4MappedToIPv6)
                    parsedIp = parsedIp.MapToIPv4();

                if (!IsPublic(parsedIp))
                {
                    Console.WriteLine($"Not a public IP :: {parsedIp}");
                    continue;
                }

                Console.WriteLine($"Using IP: {parsedIp}");
                using var reader = new DatabaseReader(DbPath);
                var country = reader.Country(parsedIp);
                Console.WriteLine($"Found ISO code: {country?.Country?.IsoCode}");
                return country?.Country?.IsoCode;
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed parsing IP {ip}: {err.Message}");
            }
        }

        return null;
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