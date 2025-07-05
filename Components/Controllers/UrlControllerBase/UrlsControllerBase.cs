using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace UrlSamurai.Components.Controllers.UrlControllerBase;

public class UrlsControllerBase : ControllerBase
{
    private readonly Regex _urlRegex = new(@"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$", RegexOptions.Compiled);
    
    protected string? GetClientIp(HttpContext context)
    {
        var headers = context.Request.Headers;
        var ip = headers["X-Forwarded-For"].FirstOrDefault()
                 ?? headers["X-Real-IP"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString();

        if (IPAddress.TryParse(ip, out var parsed) && parsed.IsIPv4MappedToIPv6)
            ip = parsed.MapToIPv4().ToString();

        return ip;
    }
    
    protected bool IsUrlValid(string? url)
    {
        return !string.IsNullOrWhiteSpace(url) && _urlRegex.IsMatch(url);
    }
}