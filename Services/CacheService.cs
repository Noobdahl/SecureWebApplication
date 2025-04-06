using Microsoft.Extensions.Caching.Memory;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetProducts(out List<string> products)
    {
        if (_cache.TryGetValue("Products", out products))
        {
            return true;
        }

        // If not in cache, fetch new products
        products = FetchNewProducts();
        return false;
    }

    private List<string> FetchNewProducts()
    {
        return _cache.GetOrCreate("Products", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
            return new List<string> { "Apple", "Pear", "Banana" }; // Simulated database fetch
        });
    }
}