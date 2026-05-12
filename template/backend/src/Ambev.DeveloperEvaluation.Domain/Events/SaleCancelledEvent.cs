using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event triggered when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent : INotification
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
}
