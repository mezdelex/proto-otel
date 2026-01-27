namespace Domain.Cache;

public interface IRedisCache
{
    string GenerateKey(params object?[] parameters);
    Task<T?> GetCachedData<T>(string key);
    Task SetCachedData<T>(string key, T value, TimeSpan timeSpan, params string[] tags)
        where T : class;
    Task RemoveKeysByTags(params string[] tags);
}
