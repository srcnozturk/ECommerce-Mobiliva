namespace ECommerceAPI.Core.Entities;

public class OrderDetailEntity
{
    public Guid Id { get; set; }
    public OrderEntity  OrderEntity { get; set; }
    public Guid  OrderEntityId { get; set; }
    public ProductEntity ProductEntity { get; set; }
    public Guid ProductEntityId { get; set; }
    public decimal UnitPrice { get; set; }
}
