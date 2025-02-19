using ECommerceAPI.Core.Interfaces;

namespace ECommerceAPI.Infrastructure.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
}
