using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event triggered when a sale item is cancelled.
/// </summary>
public class ItemCancelledEvent : INotification
{
    public Guid SaleId { get; set; }
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
}
