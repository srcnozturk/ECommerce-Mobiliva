namespace ECommerceAPI.Core.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerGSM { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<OrderDetailEntity> OrderDetails { get; set; }
}
