using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

/// <summary>
/// Handlers for Sale domain events
/// </summary>
public class SaleEventHandlers : 
    INotificationHandler<SaleCreatedEvent>,
    INotificationHandler<SaleModifiedEvent>,
    INotificationHandler<SaleCancelledEvent>,
    INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<SaleEventHandlers> _logger;

    public SaleEventHandlers(ILogger<SaleEventHandlers> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale created: ID {SaleId}, Number {SaleNumber}, Total {TotalAmount}", 
            notification.SaleId, notification.SaleNumber, notification.TotalSaleAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale modified: ID {SaleId}, Number {SaleNumber}, Total {TotalAmount}", 
            notification.SaleId, notification.SaleNumber, notification.TotalSaleAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale cancelled: ID {SaleId}, Number {SaleNumber}", 
            notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale item cancelled: SaleID {SaleId}, ItemID {SaleItemId}, ProductID {ProductId}", 
            notification.SaleId, notification.SaleItemId, notification.ProductId);
        return Task.CompletedTask;
    }
}
