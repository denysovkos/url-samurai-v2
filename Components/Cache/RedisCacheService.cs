using StackExchange.Redis;

namespace UrlSamurai.Components.Cache;

public class RedisCacheService(IConnectionMultiplexer redis)
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task SetAsync(string key, string value, TimeSpan? ttl = null)
    {
        ttl ??= TimeSpan.FromDays(14);
        await _db.StringSetAsync(key, value, ttl);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }
}