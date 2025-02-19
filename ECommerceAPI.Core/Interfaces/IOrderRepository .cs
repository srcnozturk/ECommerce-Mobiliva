using ECommerceAPI.Core.Entities;

namespace ECommerceAPI.Core.Interfaces;

public interface IOrderRepository
{
    Task<OrderEntity> GetOrderWithDetailsAsync(Guid id);
    Task<IEnumerable<OrderEntity>> GetOrdersByCustomerEmailAsync(string email);
}
