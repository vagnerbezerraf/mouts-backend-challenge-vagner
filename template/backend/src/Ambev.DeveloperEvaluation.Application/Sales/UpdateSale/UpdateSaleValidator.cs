using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand
/// </summary>
public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(sale => sale.Id).NotEmpty();
        RuleFor(sale => sale.CustomerId).NotEmpty();
        RuleFor(sale => sale.BranchId).NotEmpty();
        RuleFor(sale => sale.Items).NotEmpty();
        RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}

public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
        RuleFor(item => item.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
