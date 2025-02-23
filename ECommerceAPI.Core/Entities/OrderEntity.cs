namespace ECommerceAPI.Core.Entities;

/// <summary>
/// Represents an order in the e-commerce system.
/// Contains customer information and references to order details.
/// </summary>
public class OrderEntity
{
    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the customer who placed the order
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Email address of the customer
    /// </summary>
    public string CustomerEmail { get; set; }

    /// <summary>
    /// GSM number of the customer
    /// </summary>
    public string CustomerGSM { get; set; }

    /// <summary>
    /// Total amount of the order
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Collection of order details containing product information
    /// </summary>
    public ICollection<OrderDetailEntity> OrderDetails { get; set; }
}
