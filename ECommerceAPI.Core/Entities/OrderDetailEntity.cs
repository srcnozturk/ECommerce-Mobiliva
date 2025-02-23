namespace ECommerceAPI.Core.Entities;

/// <summary>
/// Represents a detail line item in an order.
/// Contains information about the product, quantity, and pricing.
/// </summary>
public class OrderDetailEntity
{
    /// <summary>
    /// Unique identifier for the order detail
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the parent order
    /// </summary>
    public Guid OrderEntityId { get; set; }

    /// <summary>
    /// Reference to the ordered product
    /// </summary>
    public Guid ProductEntityId { get; set; }

    /// <summary>
    /// Unit price of the product at the time of order
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Navigation property to the parent order
    /// </summary>
    public OrderEntity OrderEntity { get; set; }

    /// <summary>
    /// Navigation property to the product
    /// </summary>
    public ProductEntity ProductEntity { get; set; }
}
