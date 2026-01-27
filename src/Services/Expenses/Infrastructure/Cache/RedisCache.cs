namespace Infrastructure.Cache;

public sealed class RedisCache(IDatabase redisDatabase) : IRedisCache
{
    public string GenerateKey(params object?[] parameters) =>
        Convert.ToHexString(
            parameters
                .Aggregate(
                    new XxHash64(),
                    (acc, next) =>
                    {
                        acc.Append(Encoding.UTF8.GetBytes(next?.ToString() ?? string.Empty));

                        return acc;
                    }
                )
                .GetHashAndReset()
        );

    public async Task<T?> GetCachedData<T>(string key)
    {
        var value = await redisDatabase.StringGetAsync(key);

        return value.HasValue ? JsonSerializer.Deserialize<T>(value.ToString()) : default;
    }

    public async Task SetCachedData<T>(string key, T value, TimeSpan timeSpan, params string[] tags)
        where T : class
    {
        await redisDatabase.StringSetAsync(
            key,
            JsonSerializer.Serialize(value, value.GetType()),
            timeSpan
        );

        await Task.WhenAll(tags.Select(x => redisDatabase.SetAddAsync(x, key)));
    }

    public async Task RemoveKeysByTags(params string[] tags) =>
        await Task.WhenAll(
            tags.Select(async x =>
                await redisDatabase.KeyDeleteAsync([
                    .. (await redisDatabase.SetMembersAsync(x)).Select(key => key.ToString()),
                ])
            )
        );
}
