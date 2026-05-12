using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        // We update properties. In a real scenario we'd do this carefully, perhaps replacing the Items list entirely
        // for simplicity, we map over the properties we want to update.
        existingSale.BranchId = command.BranchId;
        existingSale.BranchName = command.BranchName;
        existingSale.CustomerId = command.CustomerId;
        existingSale.CustomerName = command.CustomerName;
        existingSale.SaleDate = command.SaleDate;

        existingSale.Items.Clear();
        foreach (var itemCmd in command.Items)
        {
            var item = _mapper.Map<SaleItem>(itemCmd);
            item.SaleId = existingSale.Id;
            existingSale.Items.Add(item);
        }

        decimal totalSaleAmount = 0;

        foreach (var item in existingSale.Items)
        {
            item.Discount = 0;
            
            if (item.Quantity >= 10 && item.Quantity <= 20)
            {
                item.Discount = 0.20m;
            }
            else if (item.Quantity >= 4 && item.Quantity < 10)
            {
                item.Discount = 0.10m;
            }

            var amountBeforeDiscount = item.Quantity * item.UnitPrice;
            item.TotalAmount = amountBeforeDiscount - (amountBeforeDiscount * item.Discount);
            totalSaleAmount += item.TotalAmount;
        }

        existingSale.TotalSaleAmount = totalSaleAmount;

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);

        await _mediator.Publish(new SaleModifiedEvent
        {
            SaleId = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            TotalSaleAmount = updatedSale.TotalSaleAmount
        }, cancellationToken);

        return result;
    }
}
