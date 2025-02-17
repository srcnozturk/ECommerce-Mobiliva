namespace ECommerceAPI.Core.Entities;

public class ProductDetailDto
{
    public Guid ProjectId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}
