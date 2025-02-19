using ECommerceAPI.Core.Entities;

namespace ECommerceAPI.Core.Interfaces;

public interface IProductRepository
{
    Task<List<ProductEntity>> GetAllAsync();
    Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
    Task<IEnumerable<ProductEntity>> GetActiveProductsAsync();
}
