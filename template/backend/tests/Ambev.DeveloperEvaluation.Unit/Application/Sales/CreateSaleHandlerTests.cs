using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();

        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Should apply 10% discount for 4 items")]
    public async Task Handle_With4Items_ShouldApply10PercentDiscount()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 4, UnitPrice = 10 }
            }
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = command.CustomerId,
            BranchId = command.BranchId,
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = command.Items[0].ProductId, Quantity = 4, UnitPrice = 10 }
            }
        };

        _mapper.Map<Sale>(command).Returns(sale);
        
        var createdSale = new Sale
        {
            Id = sale.Id,
            SaleNumber = "TEST1234",
            TotalSaleAmount = 36 // 4 * 10 = 40, 10% discount = 4, Total = 36
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(new CreateSaleResult { Id = sale.Id, TotalSaleAmount = 36 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalSaleAmount.Should().Be(36);
        sale.Items.First().Discount.Should().Be(0.10m);
        sale.Items.First().TotalAmount.Should().Be(36);
        sale.TotalSaleAmount.Should().Be(36);

        await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should apply 20% discount for 10 items")]
    public async Task Handle_With10Items_ShouldApply20PercentDiscount()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 10 }
            }
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem { ProductId = command.Items[0].ProductId, Quantity = 10, UnitPrice = 10 }
            }
        };

        _mapper.Map<Sale>(command).Returns(sale);
        
        var createdSale = new Sale
        {
            Id = sale.Id,
            SaleNumber = "TEST1234",
            TotalSaleAmount = 80 // 10 * 10 = 100, 20% discount = 20, Total = 80
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(new CreateSaleResult { Id = sale.Id, TotalSaleAmount = 80 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalSaleAmount.Should().Be(80);
        sale.Items.First().Discount.Should().Be(0.20m);
        sale.Items.First().TotalAmount.Should().Be(80);
        sale.TotalSaleAmount.Should().Be(80);
    }
}
