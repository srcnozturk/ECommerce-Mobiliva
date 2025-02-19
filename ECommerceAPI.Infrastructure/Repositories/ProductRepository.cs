using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<ProductEntity>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<List<ProductEntity>> GetAllAsync()
    {
        return await _context.Products
            .Where(p => p.Status)  // Sadece aktif ürünleri getir
            .OrderByDescending(p => p.CreateDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category == category && p.Status)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.Status)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Description)
            .ToListAsync();
    }

    // Cache kullanımı için örnek method
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
