namespace ECommerceAPI.Core.Entities;

/// <summary>
/// Represents a product in the e-commerce system.
/// Contains product information including pricing, category, and availability.
/// </summary>
public class ProductEntity
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Detailed description of the product
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Category or classification of the product
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Unit of measurement for the product
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Indicates whether the product is active and available for sale
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime? UpdateDate { get; set; }
}
