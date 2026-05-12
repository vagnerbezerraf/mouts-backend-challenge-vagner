using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event triggered when a sale is modified.
/// </summary>
public class SaleModifiedEvent : INotification
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public decimal TotalSaleAmount { get; set; }
}
