namespace ECommerceAPI.Core.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public bool Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
