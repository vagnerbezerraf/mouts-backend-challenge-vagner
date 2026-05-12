using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Navigation property to the Sale
    /// </summary>
    public Sale Sale { get; set; } = null!;

    public SaleItem()
    {
    }

    /// <summary>
    /// Cancels this item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }
}
