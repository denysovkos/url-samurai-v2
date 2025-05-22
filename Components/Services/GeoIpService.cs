namespace UrlSamurai.Components.Services;

using MaxMind.GeoIP2;

public static class GeoIpService
{
    private static readonly string DbPath = Path.Combine(
        Path.GetDirectoryName(typeof(GeoIpService).Assembly.Location)!,
        "GeoLite2-Country.mmdb"
    );

    public static string? GetCountry(string? ip)
    {
        Console.WriteLine($">>> IP {ip}");
        if (string.IsNullOrWhiteSpace(ip))
            return null;

        try
        {
            using var reader = new DatabaseReader(DbPath);
            var country = reader.Country(System.Net.IPAddress.Parse(ip));
            return country?.Country?.IsoCode;
        }
        catch
        {
            return null;
        }
    }
}