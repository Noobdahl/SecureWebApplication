using Microsoft.Extensions.Caching.Memory;

public class CacheService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _dbContext;

    public CacheService(IMemoryCache cache, AppDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public bool TryGetProducts(out List<Product> products)
    {
        if (_cache.TryGetValue("Products", out products))
        {
            return true;
        }

        // If not in cache, fetch new products
        products = FetchNewProducts()?.ToList() ?? new List<Product>();
        return false;
    }

    private List<Product>? FetchNewProducts()
    {
        return _cache.GetOrCreate("Products", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
            return FetchProductsFromDatabase();
        });
    }

    private List<Product>? FetchProductsFromDatabase()
    {
        return _dbContext.Products.ToList();
    }
}