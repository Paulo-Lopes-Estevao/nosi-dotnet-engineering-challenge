using Microsoft.Extensions.Caching.Memory;

namespace NOS.Engineering.Challenge.Cache;

public class MemoryCacheService<T> : ICacheService<T>
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await Task.FromResult(_cache.Get<T>(id));
    }

    public async Task SetAsync(Guid id, T item)
    {
        _cache.Set(id, item);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id)
    {
        _cache.Remove(id);
        await Task.CompletedTask;
    }
}
