using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UrlSamurai;

public class SwaggerFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var toRemove = swaggerDoc.Paths
            .Where(p => p.Key.StartsWith("/Account", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var path in toRemove)
        {
            swaggerDoc.Paths.Remove(path.Key);
        }
    }
}
