using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="mediator">The mediator instance</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = _mapper.Map<Sale>(command);
        sale.SaleNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); // Or any generation logic

        decimal totalSaleAmount = 0;

        foreach (var item in sale.Items)
        {
            item.Discount = 0;
            
            // Apply business rules for discounts
            if (item.Quantity >= 10 && item.Quantity <= 20)
            {
                item.Discount = 0.20m; // 20% discount
            }
            else if (item.Quantity >= 4 && item.Quantity < 10)
            {
                item.Discount = 0.10m; // 10% discount
            }

            var amountBeforeDiscount = item.Quantity * item.UnitPrice;
            item.TotalAmount = amountBeforeDiscount - (amountBeforeDiscount * item.Discount);
            totalSaleAmount += item.TotalAmount;
        }

        sale.TotalSaleAmount = totalSaleAmount;

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        var result = _mapper.Map<CreateSaleResult>(createdSale);

        // Publish event
        await _mediator.Publish(new SaleCreatedEvent
        {
            SaleId = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalSaleAmount = createdSale.TotalSaleAmount
        }, cancellationToken);

        return result;
    }
}
