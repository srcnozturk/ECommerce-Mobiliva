using ECommerceAPI.Core.Interfaces;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Infrastructure.Repositories;

namespace ECommerceAPI.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProductRepository _productRepository;
    private IOrderRepository _orderRepository;
    private bool disposed = false;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products
    {
        get
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository(_context);
            }
            return _productRepository;
        }
    }

    public IOrderRepository Orders
    {
        get
        {
            if (_orderRepository == null)
            {
                _orderRepository = new OrderRepository(_context);
            }
            return _orderRepository;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
