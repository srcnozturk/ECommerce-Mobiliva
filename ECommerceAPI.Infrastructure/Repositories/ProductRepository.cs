namespace ECommerceAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing product entities
/// </summary>
public class ProductRepository : GenericRepository<ProductEntity>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves all active products ordered by creation date
    /// </summary>
    /// <returns>A list of active products</returns>
    public async Task<List<ProductEntity>> GetAllAsync()
    {
        return await _context.Products
            .Where(p => p.Status)  // Get only active products
            .OrderByDescending(p => p.CreateDate)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves active products by category
    /// </summary>
    /// <param name="category">The category to filter by</param>
    /// <returns>A collection of active products in the specified category</returns>
    public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category == category && p.Status)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all active products ordered by category and description
    /// </summary>
    /// <returns>A collection of active products</returns>
    public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.Status)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Description)
            .ToListAsync();
    }

    /// <summary>
    /// Example method demonstrating cache usage for product retrieval
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="cacheService">Cache service for storing and retrieving products</param>
    /// <returns>A collection of products, either from cache or database</returns>
    public async Task<IEnumerable<ProductEntity>> GetProductsWithCacheAsync(
        string category,
        ICacheService cacheService)
    {
        string cacheKey = $"products-{category ?? "all"}";
        var products = cacheService.Get<IEnumerable<ProductEntity>>(cacheKey);

        if (products == null)
        {
            products = category == null
                ? await GetActiveProductsAsync()
                : await GetByCategoryAsync(category);

            cacheService.Set(cacheKey, products, TimeSpan.FromHours(1));
        }

        return products;
    }
}
