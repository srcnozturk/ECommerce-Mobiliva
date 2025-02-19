using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<OrderEntity>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<OrderEntity> GetOrderWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductEntity)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<OrderEntity>> GetOrdersByCustomerEmailAsync(string email)
    {
        return await _dbSet
            .Include(o => o.OrderDetails)
            .Where(o => o.CustomerEmail == email)
            .ToListAsync();
    }

    public async Task<OrderEntity> CreateOrderWithDetailsAsync(OrderEntity order)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _dbSet.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
