using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event triggered when a sale is created.
/// </summary>
public class SaleCreatedEvent : INotification
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public decimal TotalSaleAmount { get; set; }
}
